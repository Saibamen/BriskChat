using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IAuthenticateUser : IAction
    {
        UserModel Invoke(string email, string password, string domainName);
    }
}