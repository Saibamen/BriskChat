using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.UserDomain.Interfaces
{
    public interface IAddUserToDomain : IAction
    {
        bool Invoke(Guid userId, Guid domainId, Guid roleId);
    }
}