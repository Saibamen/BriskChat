using System;

namespace BriskChat.Web.Models.Room
{
    public class BrowseRoomsViewModel
    {
        public Guid Id { get; set; }

        public string Owner { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public string CreatedOn { get; set; }
    }
}