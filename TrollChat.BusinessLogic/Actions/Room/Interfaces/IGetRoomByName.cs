using System;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IGetRoomByName : IAction
    {
        RoomModel Invoke(string roomName, Guid domainId);
    }
}