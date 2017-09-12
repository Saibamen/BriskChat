using System.ComponentModel.DataAnnotations;

namespace TrollChat.Web.Models.Auth
{
    public class CreateDomainViewModel
    {
        [Required]
        [Display(Name = "Domain name")]
        public string DomainName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Name { get; set; }
    }
}