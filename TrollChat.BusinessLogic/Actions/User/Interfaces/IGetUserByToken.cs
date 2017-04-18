using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserByToken : IAction
    {
        UserModel Invoke(string token);
    }
}