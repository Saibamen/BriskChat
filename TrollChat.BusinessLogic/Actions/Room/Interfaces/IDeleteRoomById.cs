using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IDeleteRoomById : IAction
    {
        bool Invoke(Guid roomId);
    }
}