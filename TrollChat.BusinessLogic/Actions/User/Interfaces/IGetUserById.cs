using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserById : IAction
    {
        DataAccess.Models.User Invoke(int id);
    }
}