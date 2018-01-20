using System;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IGetRoomById : IAction
    {
        RoomModel Invoke(Guid roomId);
    }
}