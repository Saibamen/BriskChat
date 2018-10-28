using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Domain.Interfaces
{
    public interface IGetDomainByName : IAction
    {
        DomainModel Invoke(string name);
    }
}