using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IEditRoomName : IAction
    {
        bool Invoke(Guid roomId, string roomName);
    }
}