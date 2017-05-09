using System;

namespace TrollChat.Web.Models.Room
{
    public class UserConnection
    {
        public Guid UserId { get; set; }

        public string ConnectionId { get; set; }

        public Guid DomainId { get; set; }
    }
}