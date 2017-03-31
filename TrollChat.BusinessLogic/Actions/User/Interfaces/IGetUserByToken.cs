using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserByToken : IAction
    {
        DataAccess.Models.User Invoke(string token);
    }
}