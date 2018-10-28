using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface ICheckDomainExistsByName : IAction
    {
        bool Invoke(string name);
    }
}