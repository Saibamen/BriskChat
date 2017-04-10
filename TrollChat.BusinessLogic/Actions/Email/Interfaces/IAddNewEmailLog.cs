using MimeKit;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Email.Interfaces
{
    public interface IAddNewEmailLog : IAction
    {
        void Invoke(MimeMessage email);
    }
}