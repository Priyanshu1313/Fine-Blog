using FineBlog2.Data;
using FineBlog2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FineBlog2.Utiltities
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        ////public void Initialize()
        ////{
        ////    if (!_roleManager.RoleExistsAsync(WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult())
        ////    {
        ////        _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAdmin)).GetAwaiter().GetResult();
        ////        _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAuthor)).GetAwaiter().GetResult();
        ////        _userManager.CreateAsync(new ApplicationUser()
        ////        {
        ////            UserName = "admin@gmail.com",
        ////            Email = "admin@gmail.com",
        ////            FirstName = "Super",
        ////            LastName = "Admin"
        ////        }, "Admin@123").Wait();

        ////        var appUser = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@gmail.com");
        ////        if (appUser != null)
        ////        {
        ////            _userManager.AddToRoleAsync(appUser, WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult();
        ////        }

        ////        var listOfPages = new List<Page>()
        ////        {
        ////            new Page()
        ////            {
        ////                Title = "About Us",
        ////                Slug = "about"
        ////            },
        ////            new Page()
        ////            {
        ////                Title = "Contact Us",
        ////                Slug = "contact"
        ////            },
        ////            new Page()
        ////            {
        ////                Title = "Privacy Policy",
        ////                Slug = "privacy"
        ////            }
        ////        };

        ////        _context.Pages.AddRange(listOfPages);
        ////        _context.SaveChanges();
        ////    }
        ////}
        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {



            // 1) Ensure roles exist


            if (!_roleManager.RoleExistsAsync(WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult())
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAdmin)).GetAwaiter().GetResult();

            if (!_roleManager.RoleExistsAsync(WebsiteRoles.WebsiteAuthor).GetAwaiter().GetResult())
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAuthor)).GetAwaiter().GetResult();

            // 2) Ensure admin user exists
            var admin = _userManager.FindByEmailAsync("admin@gmail.com").GetAwaiter().GetResult();
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "Super",
                    LastName = "Admin"
                };
                var createResult = _userManager.CreateAsync(admin, "Admin@123").GetAwaiter().GetResult();
                if (createResult.Succeeded)
                    _userManager.AddToRoleAsync(admin, WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult();
            }

            // 3) Seed your Pages table only if empty
            if (!_context.Pages.Any())
            {
                _context.Pages.AddRange(new[] {
            new Page { Title = "About Us",      Slug = "about"   },
            new Page { Title = "Contact Us",    Slug = "contact" },
            new Page { Title = "Privacy Policy",Slug = "privacy" }
        });
                _context.SaveChanges();
            }
        }

    }
}
