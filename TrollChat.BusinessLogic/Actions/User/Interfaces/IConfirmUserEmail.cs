using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IConfirmUserEmail : IAction
    {
        bool Invoke(string userId);
    }
}