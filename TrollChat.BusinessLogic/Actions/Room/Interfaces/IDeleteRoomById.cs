using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IDeleteRoomById : IAction
    {
        bool Invoke(int roomId);
    }
}