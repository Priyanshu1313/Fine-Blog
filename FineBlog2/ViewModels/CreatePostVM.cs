﻿using System.ComponentModel.DataAnnotations;

namespace FineBlog2.ViewModels
{
    public class CreatePostVM
    {
        
            public int Id { get; set; }
            [Required]
            public string? Title { get; set; }
            public string? ShortDescription { get; set; }
            public string? ApplicationUserId { get; set; }
            public string? Description { get; set; }
            public string? ThumbnailUrl { get; set; }
            public IFormFile? Thumbnail { get; set; }

            public DateTime? CreatedDate { get; set; }
            public string? AuthorName { get; set; }



    }
}
