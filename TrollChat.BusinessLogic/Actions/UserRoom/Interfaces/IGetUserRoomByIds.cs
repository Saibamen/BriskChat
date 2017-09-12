using System;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.UserRoom.Interfaces
{
    public interface IGetUserRoomByIds : IAction
    {
        UserRoomModel Invoke(Guid roomId, Guid userId);
    }
}