using System;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface IAddNewDomain : IAction
    {
        Guid Invoke(DomainModel domain, Guid? userId = null);
    }
}