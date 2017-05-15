using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrollChat.Web.Models.Room
{
    public class RoomUsersViewModel
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}