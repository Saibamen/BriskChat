using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.UserDomain.Interfaces
{
    public interface IAddUserToDomain : IAction
    {
        bool Invoke(Guid userId, Guid domainId, Guid roleId);
    }
}