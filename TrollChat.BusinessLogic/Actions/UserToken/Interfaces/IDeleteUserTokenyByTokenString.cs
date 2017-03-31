using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IDeleteUserTokenyByTokenString : IAction
    {
        bool Invoke(string token);
    }
}