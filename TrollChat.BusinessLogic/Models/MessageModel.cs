namespace TrollChat.BusinessLogic.Models
{
    public class MessageModel : BaseModel
    {
        public UserRoomModel UserRoom { get; set; }

        public int LastMessageForId { get; set; }

        public UserRoomModel LastMessageFor { get; set; }

        public string Text { get; set; }

        public override bool IsValid()
        {
            return !(string.IsNullOrEmpty(Text));
        }
    }
}