using System;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IGetRoomByName : IAction
    {
        RoomModel Invoke(string roomName, Guid domainId);
    }
}