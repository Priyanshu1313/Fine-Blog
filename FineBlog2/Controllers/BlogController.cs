using AspNetCoreHero.ToastNotification.Abstractions;
using FineBlog2.Data;
using FineBlog2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FineBlog2.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;

        public BlogController(ApplicationDbContext context, INotyfService notification)
        {
            _context = context;
            _notification = notification;
        }
        public IActionResult Post(string slug)
        {
            if(slug == "")
            {
                _notification.Error("Post not found");
                return View();
            }
            var post = _context.Posts!.FirstOrDefault(x => x.Slug == slug);
            if(post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            var vm = new BlogPostVM()
            {
                Id = post.Id,
                Title = post.Title,
                AuthorName = post.AuthorName,
                CreatedDate= post.CreatedDate,
                ThumbnailUrl = post.ThumbnailUrl,
                Description = post.Description,
                ShortDescription = post.ShortDescription,
            };
          return View(vm);
        }
    }
}
