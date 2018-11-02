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
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoomRepository _userRoomRepository;
        private readonly IDomainRepository _domainRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddNewRoom(IRoomRepository roomRepository,
            IUserRepository userRepository,
            IUserRoomRepository userRoomRepository,
            IDomainRepository domainRepository, IUnitOfWork unitOfWork)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _userRoomRepository = userRoomRepository;
            _domainRepository = domainRepository;
            _unitOfWork = unitOfWork;
        }

        public Guid Invoke(RoomModel room, Guid userId, Guid domainId)
        {
            if (!room.IsValid() || userId == Guid.Empty || domainId == Guid.Empty ||
                // Check for room name duplication in domain
                _roomRepository.FindBy(x => x.Name == room.Name && x.Domain.Id == domainId).Count() > 0)
            {
                return Guid.Empty;
            }

            var domain = _domainRepository.GetById(domainId);

            if (domain == null)
            {
                return Guid.Empty;
            }

            var user = _userRepository.GetById(userId);

            if (user == null)
            {
                return Guid.Empty;
            }

            var newRoom = AutoMapper.Mapper.Map<DataAccess.Models.Room>(room);
            newRoom.Owner = AutoMapper.Mapper.Map<DataAccess.Models.User>(user);
            newRoom.Domain = AutoMapper.Mapper.Map<DataAccess.Models.Domain>(domain);

            _roomRepository.Add(newRoom);

            var userRoom = new DataAccess.Models.UserRoom { User = user, Room = newRoom };

            _userRoomRepository.Add(userRoom);
            _unitOfWork.Save();

            return newRoom.Id;
        }
    }
}