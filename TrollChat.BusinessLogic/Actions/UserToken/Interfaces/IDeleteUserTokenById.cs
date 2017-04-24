using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IDeleteUserTokenById : IAction
    {
        bool Invoke(Guid userTokenId);
    }
}