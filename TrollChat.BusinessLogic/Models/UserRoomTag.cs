namespace TrollChat.BusinessLogic.Models
{
    public class UserRoomTag : BaseModel
    {
        public UserRoom UserRoom { get; set; }
        public Tag Tag { get; set; }
    }
}