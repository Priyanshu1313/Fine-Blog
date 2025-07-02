using AspNetCoreHero.ToastNotification.Abstractions;
using Azure.Identity;
using FineBlog2.Data;
using FineBlog2.Models;
using FineBlog2.Utiltities;
using FineBlog2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace FineBlog2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(ApplicationDbContext context,
            INotyfService notyfService,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notification = notyfService;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //var listofPosts = new List<Post>();
            //    loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            //if (loggedInUserRole[0] = WebsiteRoles.WebsiteAdmin)
            //{
            //    var listofPosts = await _context.Posts!.Include(x => x.ApplicationUser).ToListAsync();
            //}
            //else
            //{
            //    var listofPosts = await _context.Posts!.Include(x => x.ApplicationUser).ToListAsync();
            //}
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string firstname = await _context.ApplicationUsers.Where(x => x.Id == userId).Select(x => x.FirstName).FirstOrDefaultAsync();
            string lastname = await _context.ApplicationUsers.Where(x => x.Id == userId).Select(x => x.LastName).FirstOrDefaultAsync();
            if (userId == null)
                return Unauthorized();

            // 2) load the ApplicationUser
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();

            }

            // 3) get their roles
            var roles = await _userManager.GetRolesAsync(user);
            bool isAdmin = roles.Contains(WebsiteRoles.WebsiteAdmin);

            // 4) fetch posts
            List<Post> listOfPosts;
            if (isAdmin)
            {
                // admin sees *all* posts
                listOfPosts = await _context.Posts.ToListAsync();
            }

            else
            {
                // non‑admin sees only their own
                listOfPosts = await _context.Posts
                    .Where(p => p.ApplicationUserId == userId).ToListAsync();
            }

            // return View(listOfPosts);
            var listofPostVM = listOfPosts.Select(p => new PostVM()
            {
                Id = p.Id,
                Title = p.Title,
                CreatedDate = p.CreatedDate,
                ThumbnailUrl = p.ThumbnailUrl,
                AuthorName = firstname + " " + lastname,
            }).ToList();
            return View(listofPostVM);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePostVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVM model)
        {
            if (!ModelState.IsValid)
            {
                return View((model));
            }

            //get logged in user id

            var loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(loggedInUser))
            {
                return Unauthorized();
            }

            var post = new Post();
            post.Title = model.Title;
            post.ShortDescription = model.ShortDescription;
            post.Description = model.Description;
            post.ApplicationUserId = loggedInUser;

            if (post.Title != null)
            {
                string slug = model.Title.Trim();
                slug = slug.Replace(" ", "-");
                post.Slug = slug + "-" + Guid.NewGuid();
            }

            if (model.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(model.Thumbnail);
            }

            await _context.Posts!.AddAsync(post);
            await _context.SaveChangesAsync();
            _notification.Success("Post created successfull");

            return RedirectToAction("Index");

        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();

            }
            var roles = await _userManager.GetRolesAsync(user);
            bool isAdmin = roles.Contains(WebsiteRoles.WebsiteAdmin);

            if (isAdmin || userId == post!.ApplicationUserId)
            {
                _context.Posts!.Remove(post!);
                await _context.SaveChangesAsync();
                _notification.Success("Post Deleted Successfully");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit (int id)
        {
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == id);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();

            }
            var roles = await _userManager.GetRolesAsync(user);
            bool isAdmin = roles.Contains(WebsiteRoles.WebsiteAdmin);

            if(!isAdmin || userId != post.ApplicationUserId)
            {
                _notification.Error("You are not authorized");
                return View();
            }
           

                var vm = new CreatePostVM()
                {
                    Id = post.Id,
                    Title = post.Title,
                    ShortDescription = post.ShortDescription,
                    ApplicationUserId = post.ApplicationUserId,
                    Description = post.Description,
                    ThumbnailUrl = post.ThumbnailUrl,
                    CreatedDate = post.CreatedDate

                };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            post.Title = vm.Title;
            post.ShortDescription = vm.ShortDescription;
            post.Description = vm.Description;

            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }

            await _context.SaveChangesAsync();
            _notification.Success("Post updated Succesfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }

        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Thumbnails");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }
}
