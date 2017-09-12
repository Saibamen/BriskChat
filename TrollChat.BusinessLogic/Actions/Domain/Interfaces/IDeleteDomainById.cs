using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface IDeleteDomainById : IAction
    {
        bool Invoke(Guid domainId);
    }
}