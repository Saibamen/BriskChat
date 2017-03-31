using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IEditUserPassword : IAction
    {
        bool Invoke(int userId, string plaintextPassword);
    }
}