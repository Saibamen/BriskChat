using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IEditRoomName : IAction
    {
        bool Invoke(Guid roomId, string roomName);
    }
}