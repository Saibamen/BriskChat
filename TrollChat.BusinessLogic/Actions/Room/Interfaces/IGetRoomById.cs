using System;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IGetRoomById : IAction
    {
        RoomModel Invoke(Guid roomId);
    }
}