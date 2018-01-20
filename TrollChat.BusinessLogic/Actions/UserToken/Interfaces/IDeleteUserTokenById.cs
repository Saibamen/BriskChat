using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IDeleteUserTokenById : IAction
    {
        bool Invoke(Guid userTokenId);
    }
}