using System.ComponentModel.DataAnnotations;

namespace TrollChat.Web.Models.Auth
{
    public class ResendConfirmationEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Please enter the email address you provided when registering.")]
        public string Email { get; set; }

        [Required]
        public string DomainName { get; set; }
    }
}