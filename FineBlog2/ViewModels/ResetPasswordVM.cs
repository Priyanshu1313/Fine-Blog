﻿using System.ComponentModel.DataAnnotations;

namespace FineBlog2.ViewModels
{
    public class ResetPasswordVM
    {
        public string Id { get; set; }

        public string? UserName { get; set; }
        [Required]
        public string? NewPassword { get; set; }
        [Compare(nameof(NewPassword))]
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
