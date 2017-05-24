using System;

namespace TrollChat.Web.Models.Room
{
    public class GetRoomInformationViewModel
    {
        public string OwnerName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Topic { get; set; }

        public int Customization { get; set; }

        public bool IsPublic { get; set; }

        public bool IsPrivateConversation { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}