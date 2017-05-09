using System;

namespace TrollChat.Web.Models.Room
{
    public class PrivateConversationUserViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public bool IsOnline { get; set; }
    }
}