using System;
using System.Collections.Generic;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserRooms : IRepository
    {
        List<RoomModel> Invoke(Guid userId, bool isPrivateConversation);
    }
}