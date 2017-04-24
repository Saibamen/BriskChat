using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IDeleteRoomById : IAction
    {
        bool Invoke(Guid roomId);
    }
}