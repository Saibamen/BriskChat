using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IConfirmUserEmailByToken : IAction
    {
        bool Invoke(string userId);
    }
}