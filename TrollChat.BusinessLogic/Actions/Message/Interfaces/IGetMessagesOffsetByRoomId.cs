using System;
using System.Collections.Generic;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Message.Interfaces
{
    public interface IGetMessagesOffsetByRoomId : IAction
    {
        List<MessageModel> Invoke(Guid roomId, int loadedMessagesIteration, int limit);
    }
}