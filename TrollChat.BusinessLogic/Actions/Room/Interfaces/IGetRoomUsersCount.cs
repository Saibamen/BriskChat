using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IGetRoomUsersCount : IAction
    {
        int Invoke(Guid roomId);
    }
}