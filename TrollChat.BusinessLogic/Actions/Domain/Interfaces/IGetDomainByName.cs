using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface IGetDomainByName : IAction
    {
        DomainModel Invoke(string name);
    }
}