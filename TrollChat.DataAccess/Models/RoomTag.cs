using System.ComponentModel.DataAnnotations;

namespace TrollChat.DataAccess.Models
{
    public class RoomTag : BaseEntity
    {
        [Required]
        public Room Room { get; set; }

        [Required]
        public Tag Tag { get; set; }
    }
}