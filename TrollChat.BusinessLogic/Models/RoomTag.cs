namespace TrollChat.BusinessLogic.Models
{
    public class RoomTag : BaseModel
    {
        public Room Room { get; set; }
        public Tag Tag { get; set; }
    }
}