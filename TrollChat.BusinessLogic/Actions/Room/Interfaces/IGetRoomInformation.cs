using System;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IGetRoomInformation : IAction
    {
        RoomModel Invoke(Guid roomId);
    }
}