namespace BriskChat.BusinessLogic.Models
{
    public class RoomTagModel : BaseModel
    {
        public RoomModel Room { get; set; }

        public TagModel Tag { get; set; }
    }
}