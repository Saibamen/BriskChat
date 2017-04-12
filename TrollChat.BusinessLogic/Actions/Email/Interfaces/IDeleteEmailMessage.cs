using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.DataAccess.Models;

namespace TrollChat.BusinessLogic.Actions.Email.Interfaces
{
    public interface IDeleteEmailMessage : IAction
    {
        void Invoke(EmailMessage email);
    }
}