using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrollChat.DataAccess.Models
{
    public class UserRoom : BaseEntity
    {
        public UserRoom()
        {
            Messages = new HashSet<Message>();
        }

        public virtual ICollection<Message> Messages { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Room Room { get; set; }

        [Required]
        public Role Role { get; set; }

        public DateTime? LockedUntil { get; set; }

        public Message LastMessage { get; set; }
    }
}