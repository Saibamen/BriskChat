using System;
using System.Collections.Generic;
using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IGetDomainPublicAndUserRooms : IAction
    {
        List<RoomModel> Invoke(Guid domainId, Guid userId);
    }
}