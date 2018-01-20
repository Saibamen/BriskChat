using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IDeleteUserById : IAction
    {
        bool Invoke(Guid userId);
    }
}