using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface ISetDomainOwner : IAction
    {
        bool Invoke(Guid userId, Guid domainId);
    }
}