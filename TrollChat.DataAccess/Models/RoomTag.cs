namespace TrollChat.DataAccess.Models
{
    public class RoomTag : BaseEntity
    {
        public Room Room { get; set; }
        public Tag Tag { get; set; }
    }
}