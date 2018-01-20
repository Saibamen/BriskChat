using System.ComponentModel.DataAnnotations;

namespace BriskChat.DataAccess.Models
{
    public class RoomTag : BaseEntity
    {
        [Required]
        public Room Room { get; set; }

        [Required]
        public Tag Tag { get; set; }
    }
}