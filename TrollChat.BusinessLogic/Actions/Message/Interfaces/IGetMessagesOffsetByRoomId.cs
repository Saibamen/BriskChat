using System;
using System.Collections.Generic;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Message.Interfaces
{
    public interface IGetMessagesOffsetByRoomId : IAction
    {
        List<MessageModel> Invoke(Guid roomId, Guid lastMessageId, int limit);
    }
}