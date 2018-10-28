using System.Collections.Generic;

namespace BriskChat.BusinessLogic.Models
{
    public class TagModel : BaseModel
    {
        public List<UserRoomTagModel> UserRoom { get; set; }
        public List<RoomTagModel> Room { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}