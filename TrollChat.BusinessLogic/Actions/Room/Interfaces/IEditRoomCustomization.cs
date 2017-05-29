using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IEditRoomCustomization : IAction
    {
        bool Invoke(Guid roomId, int roomCustomization);
    }
}