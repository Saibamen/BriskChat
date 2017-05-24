using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IGetRoomInformation : IAction
    {
        RoomModel Invoke(Guid roomId);
    }
}