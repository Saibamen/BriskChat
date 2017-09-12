using System.ComponentModel.DataAnnotations;

namespace TrollChat.DataAccess.Models
{
    public class UserDomain : BaseEntity
    {
        [Required]
        public User User { get; set; }

        [Required]
        public Domain Domain { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}