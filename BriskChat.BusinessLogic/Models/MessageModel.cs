﻿using System;

namespace BriskChat.BusinessLogic.Models
{
    public class MessageModel : BaseModel
    {
        public UserRoomModel UserRoom { get; set; }

        public Guid LastMessageForId { get; set; }

        public UserRoomModel LastMessageFor { get; set; }

        public string Text { get; set; }

        public override bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Text) || UserRoom == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(Text.Trim());
        }
    }
}