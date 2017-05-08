using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Message.Interfaces
{
    public interface IEditMessageById : IAction
    {
        bool Invoke(Guid messageId, string messageText);
    }
}