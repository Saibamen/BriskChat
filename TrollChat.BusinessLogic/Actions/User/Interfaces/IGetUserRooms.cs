using System;
using System.Collections.Generic;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserRooms : IAction
    {
        List<RoomModel> Invoke(Guid userId, bool isPrivateConversation);
    }
}