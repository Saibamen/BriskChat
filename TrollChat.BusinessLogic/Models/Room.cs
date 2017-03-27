using System.Collections.Generic;

namespace TrollChat.BusinessLogic.Models
{
    public class Room : BaseModel
    {
        public User Owner { get; set; }

        public List<Tag> Tags { get; set; }

        public List<User> Users { get; set; }

        public List<Message> Messages { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Topic { get; set; }

        public int Customization { get; set; }

        public bool IsPublic { get; set; }

        public bool IsPrivateConversation { get; set; }
    }
}