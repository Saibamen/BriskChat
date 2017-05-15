using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class AddNewRoom : IAddNewRoom
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserRoomRepository userRoomRepository;
        private readonly IDomainRepository domainRepository;

        public AddNewRoom(IRoomRepository roomRepository,
            IUserRepository userRepository,
            IUserRoomRepository userRoomRepository,
            IDomainRepository domainRepository)
        {
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
            this.userRoomRepository = userRoomRepository;
            this.domainRepository = domainRepository;
        }

        public Guid Invoke(RoomModel room, Guid userId, Guid domainId)
        {
            if (!room.IsValid() || userId == Guid.Empty || domainId == Guid.Empty)
            {
                return Guid.Empty;
            }

            var domain = domainRepository.GetById(domainId);

            if (domain == null)
            {
                return Guid.Empty;
            }

            var user = userRepository.GetById(userId);

            if (user == null)
            {
                return Guid.Empty;
            }

            var newRoom = AutoMapper.Mapper.Map<DataAccess.Models.Room>(room);
            newRoom.Owner = AutoMapper.Mapper.Map<DataAccess.Models.User>(user);
            newRoom.Domain = AutoMapper.Mapper.Map<DataAccess.Models.Domain>(domain);

            roomRepository.Add(newRoom);
            roomRepository.Save();

            var userRoom = new DataAccess.Models.UserRoom { User = user, Room = newRoom };

            userRoomRepository.Add(userRoom);
            userRoomRepository.Save();

            return newRoom.Id;
        }
    }
}