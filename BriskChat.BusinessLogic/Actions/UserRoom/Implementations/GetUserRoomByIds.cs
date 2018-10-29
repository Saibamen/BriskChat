using System;
using System.Linq;
using AutoMapper;
using BriskChat.BusinessLogic.Actions.UserRoom.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BriskChat.BusinessLogic.Actions.UserRoom.Implementations
{
    public class GetUserRoomByIds : IGetUserRoomByIds
    {
        private readonly IUserRoomRepository _userRoomRepository;

        public GetUserRoomByIds(IUserRoomRepository userRoomRepository)
        {
            _userRoomRepository = userRoomRepository;
        }

        public UserRoomModel Invoke(Guid roomId, Guid userId)
        {
            if (roomId == Guid.Empty || userId == Guid.Empty)
            {
                return null;
            }

            var userRoom = _userRoomRepository
                .FindBy(x => x.Room.Id == roomId && x.User.Id == userId)
                .Include(x => x.User)
                .FirstOrDefault();

            if (userRoom == null)
            {
                return null;
            }

            var userRoomModel = Mapper.Map<UserRoomModel>(userRoom);

            return userRoomModel;
        }
    }
}