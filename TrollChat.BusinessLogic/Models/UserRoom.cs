using System;
using System.Collections.Generic;

namespace TrollChat.BusinessLogic.Models
{
    public class UserRoom : BaseModel
    {
        public List<Message> Messages { get; set; }

        public List<Tag> Tags { get; set; }

        public User User { get; set; }

        public Room Room { get; set; }

        public Role Role { get; set; }

        public DateTime? LockedUntil { get; set; }

        public Message LastMessage { get; set; }
    }
}