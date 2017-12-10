﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BriskChat.DataAccess.Models
{
    public class UserRoom : BaseEntity
    {
        public UserRoom()
        {
            Messages = new HashSet<Message>();
            Tags = new HashSet<Tag>();
        }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Room Room { get; set; }

        public Role Role { get; set; }

        public DateTime? LockedUntil { get; set; }

        public Message LastMessage { get; set; }
    }
}