using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IDeleteUserById : IAction
    {
        bool Invoke(Guid userId);
    }
}