using System.ComponentModel.DataAnnotations;

namespace BriskChat.Web.Models.Auth
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm New Password")]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}