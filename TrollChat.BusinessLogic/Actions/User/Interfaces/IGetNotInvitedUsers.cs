using System;
using System.Collections.Generic;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetNotInvitedUsers : IAction
    {
        List<UserModel> Invoke(Guid domainId, Guid roomId);
    }
}