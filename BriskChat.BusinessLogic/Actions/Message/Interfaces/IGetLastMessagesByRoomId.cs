using System;
using System.Collections.Generic;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Message.Interfaces
{
    public interface IGetLastMessagesByRoomId : IAction
    {
        List<MessageModel> Invoke(Guid roomId, int number);
    }
}