using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserById : IAction
    {
        UserModel Invoke(int id);
    }
}