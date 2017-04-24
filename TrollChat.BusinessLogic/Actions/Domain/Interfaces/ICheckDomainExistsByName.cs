using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface ICheckDomainExistsByName : IAction
    {
        bool Invoke(string name);
    }
}