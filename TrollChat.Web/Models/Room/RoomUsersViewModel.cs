using System;

namespace TrollChat.Web.Models.Room
{
    public class RoomUsersViewModel
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}