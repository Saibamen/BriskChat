using System.ComponentModel.DataAnnotations;

namespace TrollChat.Web.Models.Auth
{
    public class ResendConfirmationEmailViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}