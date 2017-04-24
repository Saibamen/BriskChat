using System;
using System.Linq;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.Models;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class AddNewRoom : IAddNewRoom
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserRoomRepository userRoomRepository;

        public AddNewRoom(IRoomRepository roomRepository, IUserRepository userRepository, IUserRoomRepository userRoomRepository)
        {
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
            this.userRoomRepository = userRoomRepository;
        }

        public Guid Invoke(RoomModel room, Guid userId)
        {
            if (!room.IsValid())
            {
                return Guid.Empty;
            }

            var user = userRepository.GetById(userId);

            if (user == null)
            {
                return Guid.Empty;
            }

            var newRoom = AutoMapper.Mapper.Map<DataAccess.Models.Room>(room);
            newRoom.IsPublic = room.IsPublic;
            newRoom.Owner = AutoMapper.Mapper.Map<DataAccess.Models.User>(user);

            roomRepository.Add(newRoom);
            roomRepository.Save();

            var userRoom = new UserRoom() { User = user, Room = newRoom };

            userRoomRepository.Add(userRoom);
            userRoomRepository.Save();

            return newRoom.Id;
        }
    }
}