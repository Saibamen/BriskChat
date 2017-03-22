﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TrollChat.DataAccess.Models
{
    public class UserRoom : BaseEntity
    {
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