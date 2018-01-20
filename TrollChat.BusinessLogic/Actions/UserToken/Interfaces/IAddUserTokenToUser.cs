using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IAddUserTokenToUser : IAction
    {
        string Invoke(Guid userId);
    }
}