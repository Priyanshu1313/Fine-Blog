using AspNetCoreHero.ToastNotification.Abstractions;
using FineBlog2.Data;
using FineBlog2.Models;
using FineBlog2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace FineBlog2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")] 
    public class SettingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotyfService _notification;

        public SettingController(ApplicationDbContext context, INotyfService notyfService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _notification = notyfService;
            
        }
        public async Task<IActionResult> Index()
        {
            var settings = _context.Settings.ToList();
           if( settings.Count>0)
            {
                var vm = new SettingVM()
                {
                    Id = settings[0].Id,
                    SiteName = settings[0].SiteName,
                    Title = settings[0].Title,
                    ShortDescription = settings[0].ShortDescription,
                    ThumbnailUrl = settings[0].ThumbnailUrl,
                    FacebookUrl = settings[0].FacebookUrl,
                    GithubUrl = settings[0].GithubUrl,
                    TwitterUrl = settings[0].TwitterUrl,
                };
                return View(vm);

            }
            
                var setting = new Setting()
                {
                    SiteName = "Demo Name",
                };
            
            await _context.Settings!.AddAsync(setting);
           await _context.SaveChangesAsync();
            var createdSettings = _context.Settings.ToList();
            var CreatedVm = new SettingVM()
            {
                Id = createdSettings[0].Id,
                SiteName = createdSettings[0].SiteName,
                Title = createdSettings[0].Title,
                ShortDescription = createdSettings[0].ShortDescription,
                ThumbnailUrl = createdSettings[0].ThumbnailUrl,
                FacebookUrl = createdSettings[0].FacebookUrl,
                GithubUrl = createdSettings[0].GithubUrl,
                TwitterUrl = createdSettings[0].TwitterUrl,
            };

            return View(CreatedVm);
        }
        [HttpPost]
        public async Task<IActionResult> Index(SettingVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            } 
            var setting = await _context.Settings!.FirstOrDefaultAsync(x=>x.Id == vm.Id);
            if (setting == null)
            {
                _notification.Error("Something went wrong");
                return View(vm);
            }
            setting.SiteName = vm.SiteName;
            setting.Title = vm.Title;
            setting.ShortDescription = vm.ShortDescription;
            setting.FacebookUrl = vm.FacebookUrl;
            setting.GithubUrl = vm.GithubUrl;
            setting.TwitterUrl = vm.TwitterUrl;
            if (vm.Thumbnail != null)
            {
                setting.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
             await _context.SaveChangesAsync();
            _notification.Success("Setting updated successfully");
            return RedirectToAction("Index", "Setting", new { area = "Admin" });
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
