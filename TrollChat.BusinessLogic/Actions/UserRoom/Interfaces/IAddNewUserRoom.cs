using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.UserRoom.Interfaces
{
    public interface IAddNewUserRoom : IAction
    {
        bool Invoke(Guid roomId, Guid userId);
    }
}