using System;

namespace TrollChat.Web.Models.Message
{
    public class MessageViewModel
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public string UserName { get; set; }

        public Guid UserId { get; set; }

        public string CreatedOn { get; set; }

        public string EmailHash { get; set; }
    }
}