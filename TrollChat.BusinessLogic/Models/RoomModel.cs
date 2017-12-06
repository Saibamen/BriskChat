using System.Collections.Generic;

namespace BriskChat.BusinessLogic.Models
{
    public class RoomModel : BaseModel
    {
        public UserModel Owner { get; set; }

        public DomainModel Domain { get; set; }

        public List<TagModel> Tags { get; set; }

        public List<UserModel> Users { get; set; }

        public List<MessageModel> Messages { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Topic { get; set; }

        public int Customization { get; set; }

        public bool IsPublic { get; set; }

        public bool IsPrivateConversation { get; set; }

        public override bool IsValid()
        {
            if (string.IsNullOrEmpty(Name) || Name.Length > 100 || !string.IsNullOrEmpty(Description) && Description.Length > 100 || !string.IsNullOrEmpty(Topic) && Topic.Length > 100)
            {
                return false;
            }

            return !string.IsNullOrEmpty(Name.Trim());
        }
    }
}