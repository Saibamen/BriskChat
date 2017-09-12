using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrollChat.DataAccess.Models
{
    public class Domain : BaseEntity
    {
        public Domain()
        {
            Users = new HashSet<User>();
            Rooms = new HashSet<Room>();
        }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(100)")]
        public string Name { get; set; }

        public User Owner { get; set; }
    }
}