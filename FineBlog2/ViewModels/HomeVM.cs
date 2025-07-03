using FineBlog2.Models;
using X.PagedList;

namespace FineBlog2.ViewModels
{
    public class HomeVM
    {
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ThumbnailUrl { get; set; }
        public IPagedList<Post>? Posts { get; set; } 
        public string? AuthorName { get; set; } 

    }
}
