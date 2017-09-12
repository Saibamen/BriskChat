using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IAuthenticateUser : IAction
    {
        UserModel Invoke(string email, string password, string domainName);
    }
}