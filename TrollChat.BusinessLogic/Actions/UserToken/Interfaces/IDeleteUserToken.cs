using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IDeleteUserToken : IAction
    {
        bool Invoke(int userTokenId);
    }
}