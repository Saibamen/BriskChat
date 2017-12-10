using System;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface IGetDomainByUserId : IAction
    {
        DomainModel Invoke(Guid userGuid);
    }
}