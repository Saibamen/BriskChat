using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserByToken : IAction
    {
        UserModel Invoke(string token);
    }
}