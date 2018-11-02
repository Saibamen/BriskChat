using System;
using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.UserRoom.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.UserRoom.Implementations
{
    public class AddNewUserRoom : IAddNewUserRoom
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoomRepository _userRoomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddNewUserRoom(IRoomRepository roomRepository,
            IUserRepository userRepository,
            IUserRoomRepository userRoomRepository, IUnitOfWork unitOfWork)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _userRoomRepository = userRoomRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid roomId, List<Guid> users, bool invite = false)
        {
            if (roomId == Guid.Empty || users.Count <= 0 || users.Any(x => x == Guid.Empty) ||
                // Check for existing userRoom
                _userRoomRepository.FindBy(x => x.Room.Id == roomId && users.Any(y => y == x.User.Id)).Count() > 0)
            {
                return false;
            }

            var room = _roomRepository.GetById(roomId);

            if (room == null || !room.IsPublic && !invite)
            {
                return false;
            }

            foreach (var user in users)
            {
                var userFromDb = _userRepository.GetById(user);

                if (userFromDb == null)
                {
                    return false;
                }

                var userRoomToAdd = new DataAccess.Models.UserRoom { User = userFromDb, Room = room };

                _userRoomRepository.Add(userRoomToAdd);
            }

            _unitOfWork.Save();

            return true;
        }
    }
}