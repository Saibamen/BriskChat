using System;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.UserRoom.Interfaces
{
    public interface IGetUserRoomByIds : IAction
    {
        UserRoomModel Invoke(Guid roomId, Guid userId);
    }
}