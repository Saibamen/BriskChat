using System;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
{
    public class AddNewRoom : IAddNewRoom
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserRoomRepository userRoomRepository;
        private readonly IDomainRepository domainRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddNewRoom(IRoomRepository roomRepository,
            IUserRepository userRepository,
            IUserRoomRepository userRoomRepository,
            IDomainRepository domainRepository, IUnitOfWork unitOfWork)
        {
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
            this.userRoomRepository = userRoomRepository;
            this.domainRepository = domainRepository;
            _unitOfWork = unitOfWork;
        }

        public Guid Invoke(RoomModel room, Guid userId, Guid domainId)
        {
            if (!room.IsValid() || userId == Guid.Empty || domainId == Guid.Empty ||
                // Check for room name duplication in domain
                roomRepository.FindBy(x => x.Name == room.Name && x.Domain.Id == domainId).Count() > 0)
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

            var userRoom = new DataAccess.Models.UserRoom { User = user, Room = newRoom };

            userRoomRepository.Add(userRoom);
            _unitOfWork.Save();

            return newRoom.Id;
        }
    }
}