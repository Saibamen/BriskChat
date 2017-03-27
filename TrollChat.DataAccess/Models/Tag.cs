using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrollChat.DataAccess.Models
{
    public class Tag : BaseEntity
    {
        public Tag()
        {
            UserRoom = new HashSet<UserRoomTag>();
            Room = new HashSet<RoomTag>();
        }

        public virtual ICollection<UserRoomTag> UserRoom { get; set; }
        public virtual ICollection<RoomTag> Room { get; set; }
        
        [Required]
        [Column(TypeName = "NVARCHAR(100)")]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string Description { get; set; }
    }
}