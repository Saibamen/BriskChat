using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Role.Interfaces
{
    public interface IGetRoleByName : IAction
    {
        RoleModel Invoke(string name);
    }
}