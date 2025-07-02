using AspNetCoreHero.ToastNotification.Abstractions;
using FineBlog2.Models;
using FineBlog2.Utiltities;
using FineBlog2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FineBlog2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notification;

        public UserController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            INotyfService notyfService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notification = notyfService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var vm = users.Select(x => new UserVM()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                Email = x.Email,
            }).ToList();

            foreach (var user in vm)
            {
                var singleUser = await _userManager.FindByIdAsync(user.Id);
                var role = await _userManager.GetRolesAsync(singleUser);
                user.Role = role.FirstOrDefault();
            }

            return View(vm);
        }

        public async Task<IActionResult> ResetPassword(string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                _notification.Error("User does not exist");
                return View();
            }

            var vm = new ResetPasswordVM()
            {
                Id = existingUser.Id,
                UserName = existingUser.UserName,
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var existingUser = await _userManager.FindByIdAsync(vm.Id);
            if (existingUser == null)
            {
                _notification.Error("User does not exist");
                return View(vm);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            var result = await _userManager.ResetPasswordAsync(existingUser, token, vm.NewPassword);

            if (result.Succeeded)
            {
                _notification.Success("Password reset successful");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(vm);
        }

        public IActionResult Register()
        {
            return View(new RegisterVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var checkUserByEmail = await _userManager.FindByEmailAsync(vm.Email);
            if (checkUserByEmail != null)
            {
                _notification.Error("Email already exists");
                return View(vm);
            }

            var checkUserByUsername = await _userManager.FindByNameAsync(vm.UserName);
            if (checkUserByUsername != null)
            {
                _notification.Error("Username already exists");
                return View(vm);
            }

            var applicationUser = new ApplicationUser()
            {
                Email = vm.Email,
                UserName = vm.UserName,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
            };

            var result = await _userManager.CreateAsync(applicationUser, vm.Password);
            if (result.Succeeded)
            {
                if (vm.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAdmin);
                }
                else
                {
                    await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAuthor);
                }

                _notification.Success("User registered successfully.");
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(vm);
        }

        //[Route("Admin/User/Edit/{userName}")]
        //public async Task<IActionResult> Edit(string UserName)
        //{
        //    if(string.IsNullOrEmpty(UserName))
        //    {
        //        return NotFound();
        //    }
        //    var user = await _userManager.FindByNameAsync(UserName);
        //    if(user == null)
        //    {
        //        _notification.Error("User not found.");
        //        return RedirectToAction(nameof(Index));
        //    }
        //    var roles = await _userManager.GetRolesAsync(user);
        //    var vm = new EditVM()
        //    {
                
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Role = roles.FirstOrDefault()
        //    };
        //    return View(vm);
        //}

        ////[HttpPost]
        ////[ValidateAntiForgeryToken]
        ////public async Task<IActionResult> Edit(EditVM vm)
        ////{
        ////    if (!ModelState.IsValid)
        ////    {
        ////        return View(vm);
        ////    }
        ////    var user = await _userManager.FindByNameAsync(vm.UserName);
        ////    if(user == null)
        ////    {
        ////        _notification.Error("User not found.");
        ////        return RedirectToAction(nameof(Index));
        ////    }
        ////    user.UserName = vm.UserName;
        ////    user.Email = vm.Email;
        ////    user.FirstName = vm.FirstName;
        ////    user.LastName = vm.LastName;

        ////    var result = await _userManager.UpdateAsync(user);
        ////    if(result.Succeeded)
        ////    {
        ////        var currentRoles = await _userManager.GetRolesAsync(user);
        ////        if(!currentRoles.Contains(vm.Role))
        ////        {
        ////            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        ////            var roleResult = await _userManager.AddToRoleAsync(user, vm.Role);
        ////            if(!roleResult.Succeeded)
        ////            {
        ////                foreach (var error in roleResult.Errors)
        ////                {
        ////                    ModelState.AddModelError(string.Empty, error.Description);
        ////                }
        ////                return View(vm);
        ////            }
        ////            _notification.Success("User updated successfully.");
        ////            return RedirectToAction("Index", "User", new { area = "Admin" });
        ////        }
        ////    }
        ////    foreach (var error in result.Errors)
        ////    {
        ////        ModelState.AddModelError(string.Empty, error.Description);
        ////    }
        ////    return View(vm);
        ////}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(EditVM vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(vm);
        //    }

        //    var user = await _userManager.FindByNameAsync(vm.UserName);
        //    if (user == null)
        //    {
        //        _notification.Error("User not found.");
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Update user properties
        //    user.Email = vm.Email;
        //    user.FirstName = vm.FirstName;
        //    user.LastName = vm.LastName;
        //    // Note: Don't update UserName as it can cause issues

        //    var result = await _userManager.UpdateAsync(user);
        //    if (result.Succeeded)
        //    {
        //        // Handle role update
        //        var currentRoles = await _userManager.GetRolesAsync(user);
        //        if (!string.IsNullOrEmpty(vm.Role) && !currentRoles.Contains(vm.Role))
        //        {
        //            // Remove all current roles
        //            if (currentRoles.Any())
        //            {
        //                await _userManager.RemoveFromRolesAsync(user, currentRoles);
        //            }

        //            // Add new role
        //            var roleResult = await _userManager.AddToRoleAsync(user, vm.Role);
        //            if (!roleResult.Succeeded)
        //            {
        //                foreach (var error in roleResult.Errors)
        //                {
        //                    ModelState.AddModelError(string.Empty, error.Description);
        //                }
        //                return View(vm);
        //            }
        //        }

        //        _notification.Success("User updated successfully.");
        //        return RedirectToAction("Index", "User", new { area = "Admin" });
        //    }

        //    // If we got here, something failed
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError(string.Empty, error.Description);
        //    }
        //    return View(vm);
        //}


        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginVM());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == vm.Username);
            if (existingUser == null)
            {
                _notification.Error("Username does not exist");
                return View(vm);
            }

            var verifyPassword = await _userManager.CheckPasswordAsync(existingUser, vm.Password);
            if (!verifyPassword)
            {
                _notification.Error("Password does not match");
                return View(vm);
            }

            var result = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _notification.Success("Login successful");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            else if (result.IsLockedOut)
            {
                _notification.Error("User account locked out.");
                return View(vm);
            }
            else
            {
                _notification.Error("Invalid login attempt.");
                return View(vm);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _notification.Success("You are logged out successfully");
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet("AccessDenied")]
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
       
    }
}
