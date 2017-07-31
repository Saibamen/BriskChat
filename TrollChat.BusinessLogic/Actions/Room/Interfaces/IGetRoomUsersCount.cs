using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IGetRoomUsersCount : IAction
    {
        int Invoke(Guid roomId);
    }
}