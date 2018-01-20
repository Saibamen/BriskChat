using System;
using System.Collections.Generic;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserRooms : IAction
    {
        List<RoomModel> Invoke(Guid userId, bool isPrivateConversation);
    }
}