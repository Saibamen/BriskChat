using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IAuthorizeUser : IAction
    {
        bool Invoke(string email, string password);
    }
}