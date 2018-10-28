using System.ComponentModel.DataAnnotations;

namespace BriskChat.Web.Models.Auth
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Domain name")]
        public string DomainName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}