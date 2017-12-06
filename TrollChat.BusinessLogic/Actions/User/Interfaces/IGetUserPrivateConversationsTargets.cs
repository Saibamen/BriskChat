﻿using System;
using System.Collections.Generic;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserPrivateConversationsTargets : IAction
    {
        List<UserRoomModel> Invoke(Guid userId);
    }
}