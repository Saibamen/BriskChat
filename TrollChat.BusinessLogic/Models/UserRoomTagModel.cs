namespace TrollChat.BusinessLogic.Models
{
    public class UserRoomTagModel : BaseModel
    {
        public UserRoomModel UserRoom { get; set; }

        public TagModel Tag { get; set; }
    }
}