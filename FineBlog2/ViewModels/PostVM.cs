using FineBlog2.Models;

namespace FineBlog2.ViewModels
{
    public class PostVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }   
        public string? AuthorName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string ? PostedBy { get; set; }


    }
}
