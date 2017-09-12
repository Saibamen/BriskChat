using System;
using System.Collections.Generic;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserPrivateConversationsTargets : IAction
    {
        List<UserRoomModel> Invoke(Guid userId);
    }
}