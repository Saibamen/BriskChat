using System;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Role.Interfaces
{
    public interface IAddNewRole : IAction
    {
        Guid Invoke(RoleModel model);
    }
}