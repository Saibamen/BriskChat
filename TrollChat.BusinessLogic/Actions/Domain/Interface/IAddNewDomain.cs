using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Domain.Interface
{
    public interface IAddNewDomain : IAction
    {
        int Invoke(Models.DomainModel domain);
    }
}