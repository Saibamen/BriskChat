using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface ISetDomainOwner : IAction
    {
        bool Invoke(Guid userId, Guid domainId);
    }
}