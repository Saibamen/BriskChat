using System;
using System.Collections.Generic;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IGetRoomUsers : IAction
    {
        List<UserModel> Invoke(Guid roomId);
    }
}