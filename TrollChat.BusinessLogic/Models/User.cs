using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrollChat.BusinessLogic.Models
{
    public class User : BaseModel
    {
        public List<UserRoom> Rooms { get; set; }

        public string Email { get; set; }

        public DateTime? EmailConfirmedOn { get; set; }

        public string Name { get; set; }

        public DateTime? LockedOn { get; set; }
    }
}