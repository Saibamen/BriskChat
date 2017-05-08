using System;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface IGetDomainByUserId : IAction
    {
        DomainModel Invoke(Guid userGuid);
    }
}