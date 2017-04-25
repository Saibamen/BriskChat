using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrollChat.Web.Models.Room
{
    public class PrivateConversationViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }
    }
}