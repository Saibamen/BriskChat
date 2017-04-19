using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Domain.Interface
{
    public interface IAddNewDomain : IAction
    {
        int Invoke(DomainModel domain, int userId);
    }
}