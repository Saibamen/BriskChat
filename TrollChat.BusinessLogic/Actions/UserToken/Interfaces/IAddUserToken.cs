using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IAddUserToken : IAction
    {
        bool Invoke(int userId);
    }
}