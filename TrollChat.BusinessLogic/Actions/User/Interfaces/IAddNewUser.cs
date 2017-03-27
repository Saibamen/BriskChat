using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IAddNewUser : IAction
    {
        int Invoke(Models.User user);
    }
}