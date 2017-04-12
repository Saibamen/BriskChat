using System.Collections.Generic;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.DataAccess.Models;

namespace TrollChat.BusinessLogic.Actions.Email.Interfaces
{
    public interface IGetEmailMessages : IAction
    {
        List<EmailMessage> Invoke(int count);
    }
}