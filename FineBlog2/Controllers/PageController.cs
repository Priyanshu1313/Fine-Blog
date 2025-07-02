﻿using FineBlog2.Data;
using FineBlog2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FineBlog2.Controllers
{
    public class PageController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PageController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> About()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x=>x.Slug=="about");
            var vm = new PageVM()
            {
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl,
            };
            return View(vm);
        }

        public async Task<IActionResult> Contact()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "Contact");
            var vm = new PageVM()
            {
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl,
            };
            return View(vm);
        }
        public async Task<IActionResult> PrivacyPolicy()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "Privacy");
            var vm = new PageVM()
            {
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl,
            };
            return View(vm);

        }

    }
}
