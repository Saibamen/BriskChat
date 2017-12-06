using System;
using System.Collections.Generic;

namespace BriskChat.BusinessLogic.Models
{
    public class UserModel : BaseModel
    {
        public List<RoomModel> Rooms { get; set; }

        public List<UserRoomModel> UserRooms { get; set; }

        public List<UserTokenModel> Tokens { get; set; }

        public string Email { get; set; }

        public DateTime? EmailConfirmedOn { get; set; }

        public string Password { get; set; }

        public DomainModel Domain { get; set; }

        public string Name { get; set; }

        public DateTime? LockedOn { get; set; }

        public override bool IsValid()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Name))
            {
                return false;
            }

            return !string.IsNullOrEmpty(Email.Trim()) && !string.IsNullOrEmpty(Name.Trim());
        }
    }
}