using System.ComponentModel.DataAnnotations;

namespace BriskChat.DataAccess.Models
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