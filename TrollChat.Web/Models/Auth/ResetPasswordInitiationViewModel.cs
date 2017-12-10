using System.ComponentModel.DataAnnotations;

namespace BriskChat.Web.Models.Auth
{
    public class ResetPasswordInitiationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string DomainName { get; set; }
    }
}