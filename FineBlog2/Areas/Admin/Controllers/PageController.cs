using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using FineBlog2.Data;
using FineBlog2.Models;
using FineBlog2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FineBlog2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class PageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PageController(ApplicationDbContext context,IWebHostEnvironment webHostEnvironment,INotyfService notyfService)
        {
            _context = context;
            _notification = notyfService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
       public async Task<IActionResult> About()
        {
            var aboutPage = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "about");
            var vm = new PageVM()
            {
                Id = aboutPage.Id,
                Title = aboutPage.Title,
                ShortDescription = aboutPage.ShortDescription,
                Description = aboutPage.Description,
                ThumbnailUrl = aboutPage.ThumbnailUrl,

            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> About(PageVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var Page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "about");
            if (Page == null)
            {
                _notification.Error("Page not found");
                return View();
            }
            Page.Title = vm.Title;
            Page.ShortDescription = vm.ShortDescription;
            Page.Description = vm.Description;
            if (vm.Thumbnail != null)
            {
                Page.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("About Page Updated Successfully");
            return RedirectToAction("About","Page", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            var aboutPage = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "Contact");
            var vm = new PageVM()
            {
                Id = aboutPage.Id,
                Title = aboutPage.Title,
                ShortDescription = aboutPage.ShortDescription,
                Description = aboutPage.Description,
                ThumbnailUrl = aboutPage.ThumbnailUrl,

            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Contact(PageVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var Page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "Contact");
            if (Page == null)
            {
                _notification.Error("Page not found");
                return View();
            }
            Page.Title = vm.Title;
            Page.ShortDescription = vm.ShortDescription;
            Page.Description = vm.Description;
            if (vm.Thumbnail != null)
            {
                Page.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Contact Page Updated Successfully");
            return RedirectToAction("Contact", "Page", new { area = "Admin" });
        }


        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            var aboutPage = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "Privacy");
            var vm = new PageVM()
            {
                Id = aboutPage.Id,
                Title = aboutPage.Title,
                ShortDescription = aboutPage.ShortDescription,
                Description = aboutPage.Description,
                ThumbnailUrl = aboutPage.ThumbnailUrl,

            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Privacy(PageVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var Page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "Privacy");
            if (Page == null)
            {
                _notification.Error("Page not found");
                return View();
            }
            Page.Title = vm.Title;
            Page.ShortDescription = vm.ShortDescription;
            Page.Description = vm.Description;
            if (vm.Thumbnail != null)
            {
                Page.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Privacy Page Updated Successfully");
            return RedirectToAction("Privacy", "Page", new { area = "Admin" });
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
