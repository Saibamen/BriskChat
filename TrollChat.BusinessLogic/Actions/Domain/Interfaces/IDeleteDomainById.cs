using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface IDeleteDomainById : IAction
    {
        bool Invoke(int domainId);
    }
}