using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IEditRoomTopic : IAction
    {
        bool Invoke(Guid roomId, string roomTopic);
    }
}