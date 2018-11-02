using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IDeleteUserTokenByTokenString : IAction
    {
        bool Invoke(string token);
    }
}