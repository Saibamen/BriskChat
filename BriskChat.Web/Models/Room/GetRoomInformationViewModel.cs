﻿using System;

namespace BriskChat.Web.Models.Room
{
    public class GetRoomInformationViewModel
    {
        public string Name { get; set; }

        public string OwnerName { get; set; }

        public string Description { get; set; }

        public string Topic { get; set; }

        public int Customization { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsPrivateConversation { get; set; }

        public bool IsPublic { get; set; }
    }
}