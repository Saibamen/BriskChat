using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.Message.Interfaces
{
    public interface IEditMessageById : IAction
    {
        bool Invoke(Guid messageId, string messageText);
    }
}