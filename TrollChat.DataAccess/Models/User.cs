using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrollChat.DataAccess.Models
{
    public class User : BaseEntity
    {
        public User()
        {
            Rooms = new HashSet<Room>();
            Tokens = new HashSet<UserToken>();
            UserRooms = new HashSet<UserRoom>();
        }

        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<UserRoom> UserRooms { get; set; }
        public virtual ICollection<UserToken> Tokens { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(256)")]
        public string Email { get; set; }

        [ForeignKey(nameof(Domain))]
        public Guid DomainId { get; set; }

        [Required]
        public Domain Domain { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(256)")]
        public string PasswordHash { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(128)")]
        public string PasswordSalt { get; set; }

        public DateTime? EmailConfirmedOn { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(100)")]
        public string Name { get; set; }

        public DateTime? LockedOn { get; set; }
    }
}