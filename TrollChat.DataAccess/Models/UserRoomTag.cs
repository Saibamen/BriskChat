using System.ComponentModel.DataAnnotations;

namespace BriskChat.DataAccess.Models
{
    public class UserRoomTag : BaseEntity
    {
        [Required]
        public UserRoom UserRoom { get; set; }

        [Required]
        public Tag Tag { get; set; }
    }
}