using System.ComponentModel.DataAnnotations;

namespace TrollChat.Web.Models.Auth
{
    public class Register
    {
        public class RegisterViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }
        }
    }
}