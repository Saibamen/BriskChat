using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface IDeleteDomainById : IAction
    {
        bool Invoke(Guid domainId);
    }
}