using System;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface IAddNewDomain : IAction
    {
        Guid Invoke(DomainModel domain, Guid userId);
    }
}