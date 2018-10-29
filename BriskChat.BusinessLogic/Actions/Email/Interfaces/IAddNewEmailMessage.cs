using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Email.Interfaces
{
    public interface IAddNewEmailMessage : IAction
    {
        bool Invoke(EmailMessageModel email);
    }
}