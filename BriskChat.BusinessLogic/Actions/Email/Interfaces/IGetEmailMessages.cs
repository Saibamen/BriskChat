using System.Collections.Generic;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.DataAccess.Models;

namespace BriskChat.BusinessLogic.Actions.Email.Interfaces
{
    public interface IGetEmailMessages : IAction
    {
        List<EmailMessage> Invoke(int count);
    }
}