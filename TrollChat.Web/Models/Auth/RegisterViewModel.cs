using System.ComponentModel.DataAnnotations;

namespace TrollChat.Web.Models.Auth
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(RegisterViewModel.Password))]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Name { get; set; }
    }
}