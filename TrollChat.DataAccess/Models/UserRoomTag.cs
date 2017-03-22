namespace TrollChat.DataAccess.Models
{
    public class UserRoomTag : BaseEntity
    {
        public UserRoom UserRoom { get; set; }
        public Tag Tag { get; set; }
    }
}