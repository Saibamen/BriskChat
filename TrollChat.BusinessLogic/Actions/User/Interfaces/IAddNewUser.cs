using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IAddNewUser : IAction
    {
        DataAccess.Models.User Invoke(Models.UserModel user);
    }
}