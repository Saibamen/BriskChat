using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IEditRoomCustomization : IAction
    {
        bool Invoke(Guid roomId, int roomCustomization);
    }
}