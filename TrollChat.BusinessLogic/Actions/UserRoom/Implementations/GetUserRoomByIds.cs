using System;
using System.Linq;
using AutoMapper;
using TrollChat.BusinessLogic.Actions.UserRoom.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserRoom.Implementations
{
    public class GetUserRoomByIds : IGetUserRoomByIds
    {
        private readonly IUserRoomRepository userRoomRepository;

        public GetUserRoomByIds(IUserRoomRepository userRoomRepository)
        {
            this.userRoomRepository = userRoomRepository;
        }

        public UserRoomModel Invoke(Guid roomId, Guid userId)
        {
            var userRoom = userRoomRepository.FindBy(x => x.Room.Id == roomId && x.User.Id == userId).FirstOrDefault();

            if (userRoom == null)
            {
                return null;
            }

            var userRoomModel = Mapper.Map<UserRoomModel>(userRoom);

            return userRoomModel;
        }
    }
}