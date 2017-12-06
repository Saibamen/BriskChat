using System;

namespace BriskChat.Web.Models.Room
{
    public class UserViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string EmailHash { get; set; }

        public bool IsOnline { get; set; }
    }
}