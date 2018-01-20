using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IAddNewUser : IAction
    {
        DataAccess.Models.User Invoke(Models.UserModel user);
    }
}