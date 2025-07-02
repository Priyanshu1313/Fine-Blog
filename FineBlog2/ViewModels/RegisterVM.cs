using Microsoft.Build.Framework;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace FineBlog2.ViewModels
{

    public class RegisterVM
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
