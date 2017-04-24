using System;
using System.Runtime.InteropServices;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Message.Interfaces
{
    public interface IDeleteMessageById : IAction
    {
        bool Invoke(Guid IdMessage);
    }
}