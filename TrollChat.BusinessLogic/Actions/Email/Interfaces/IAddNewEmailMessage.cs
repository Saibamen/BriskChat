using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Email.Interfaces
{
    public interface IAddNewEmailMessage : IAction
    {
        bool Invoke(EmailMessageModel email);
    }
}