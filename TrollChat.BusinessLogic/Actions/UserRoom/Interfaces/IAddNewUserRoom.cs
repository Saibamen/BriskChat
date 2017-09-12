using System;
using System.Collections.Generic;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.UserRoom.Interfaces
{
    public interface IAddNewUserRoom : IAction
    {
        bool Invoke(Guid roomId, List<Guid> users, bool invite = false);
    }
}