using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Role.Interfaces
{
    public interface IGetRoleByName : IAction
    {
        RoleModel Invoke(string name);
    }
}