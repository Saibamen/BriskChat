using System;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IAddNewPrivateConversation : IAction
    {
        Guid Invoke(RoomModel room, Guid issuerUserId, Guid secondUserId);
    }
}