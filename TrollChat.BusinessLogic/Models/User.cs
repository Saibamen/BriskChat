using System;
using System.Collections.Generic;
using TrollChat.DataAccess.Models;

namespace TrollChat.BusinessLogic.Models
{
    public class User : BaseModel
    {
        public List<UserRoom> Rooms { get; set; }

        public List<UserToken> Tokens { get; set; }

        public string Email { get; set; }

        public DateTime? EmailConfirmedOn { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public DateTime? LockedOn { get; set; }

        public override bool IsValid()
        {
            return !(string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Name));
        }
    }
}