namespace TrollChat.BusinessLogic.Models
{
    class UserRoomTag : BaseModel
    {
        public UserRoom UserRoom { get; set; }
        public Tag Tag { get; set; }
    }
}
