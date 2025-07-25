﻿using Microsoft.AspNetCore.Identity;

namespace FineBlog2.Models
{
    public class ApplicationUser:IdentityUser

    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public List<Post>? Posts { get; set; }
    }
}
