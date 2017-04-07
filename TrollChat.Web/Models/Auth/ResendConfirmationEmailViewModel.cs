using System.ComponentModel.DataAnnotations;

namespace TrollChat.Web.Models.Auth
{
    public class ResendConfirmationEmailViewModel
    {
        [Required]
        [Display(Name = "Please enter the email address you provided when registering.")]
        public string Email { get; set; }
    }
}