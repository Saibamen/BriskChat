using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IConfirmUserEmailByToken : IAction
    {
        bool Invoke(string userId);
    }
}