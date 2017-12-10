using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.Message.Interfaces
{
    public interface IDeleteMessageById : IAction
    {
        bool Invoke(Guid messageId);
    }
}