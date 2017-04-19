using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Domain.Interface
{
    public interface IDeleteDomainById : IAction
    {
        bool Invoke(int domainId);
    }
}