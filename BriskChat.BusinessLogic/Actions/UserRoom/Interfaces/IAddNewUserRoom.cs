using System;
using System.Collections.Generic;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.UserRoom.Interfaces
{
    public interface IAddNewUserRoom : IAction
    {
        bool Invoke(Guid roomId, List<Guid> users, bool invite = false);
    }
}