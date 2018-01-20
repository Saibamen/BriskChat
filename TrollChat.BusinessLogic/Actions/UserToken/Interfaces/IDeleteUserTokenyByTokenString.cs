using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IDeleteUserTokenyByTokenString : IAction
    {
        bool Invoke(string token);
    }
}