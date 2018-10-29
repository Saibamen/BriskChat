using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BriskChat.DataAccess.Models
{
    public class Room : BaseEntity
    {
        public Room()
        {
            Tags = new HashSet<Tag>();
            UserRooms = new HashSet<UserRoom>();
            //Users = new HashSet<User>();
        }

        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<UserRoom> UserRooms { get; set; }
        //public virtual ICollection<User> Users { get; set; }

        [Required]
        public User Owner { get; set; }

        [Required]
        public Domain Domain { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(100)")]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string Description { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string Topic { get; set; }

        public int Customization { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        [Required]
        public bool IsPrivateConversation { get; set; }
    }
}