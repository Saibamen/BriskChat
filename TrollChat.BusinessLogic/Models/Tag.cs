using System.Collections.Generic;

namespace TrollChat.BusinessLogic.Models
{
    class Tag : BaseModel
    {
        public List<UserRoomTag> UserRoom { get; set; }
        public List<RoomTag> Room { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
