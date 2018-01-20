using System;
using System.Collections.Generic;

namespace BriskChat.BusinessLogic.Models
{
    public class UserRoomModel : BaseModel
    {
        public List<MessageModel> Messages { get; set; }

        public List<TagModel> Tags { get; set; }

        public UserModel User { get; set; }

        public RoomModel Room { get; set; }

        public RoleModel Role { get; set; }

        public DateTime? LockedUntil { get; set; }

        public MessageModel LastMessage { get; set; }
    }
}