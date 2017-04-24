using System;

namespace TrollChat.Web.Models.Room
{
    public class CreateNewPrivateConversationViewModel
    {
        public Guid SecondUserId { get; set; }

        public string Name { get; set; }
    }
}