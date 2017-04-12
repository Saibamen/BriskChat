using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Email.Interfaces
{
    public interface IDeleteEmailMessageById : IAction
    {
        bool Invoke(int emailId);
    }
}