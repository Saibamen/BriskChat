using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.Email.Interfaces
{
    public interface IDeleteEmailMessageById : IAction
    {
        bool Invoke(Guid emailId);
    }
}