using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserByEmail : IAction
    {
        DataAccess.Models.User Invoke(string email);
    }
}