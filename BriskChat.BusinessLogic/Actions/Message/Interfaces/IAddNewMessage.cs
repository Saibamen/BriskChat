using System;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Message.Interfaces
{
    public interface IAddNewMessage : IAction
    {
        Guid Invoke(MessageModel message);
    }
}