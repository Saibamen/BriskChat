using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IEditRoomDescription : IAction
    {
        bool Invoke(Guid roomId, string roomDescription);
    }
}