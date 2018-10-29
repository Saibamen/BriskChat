using System;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Role.Interfaces
{
    public interface IAddNewRole : IAction
    {
        Guid Invoke(RoleModel model);
    }
}