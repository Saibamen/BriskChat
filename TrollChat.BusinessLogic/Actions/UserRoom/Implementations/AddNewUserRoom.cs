using System;
using System.Collections.Generic;
using System.Linq;
using TrollChat.BusinessLogic.Actions.UserRoom.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Actions.UserRoom.Implementations
{
    public class AddNewUserRoom : IAddNewUserRoom
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserRoomRepository userRoomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddNewUserRoom(IRoomRepository roomRepository,
            IUserRepository userRepository,
            IUserRoomRepository userRoomRepository, IUnitOfWork unitOfWork)
        {
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
            this.userRoomRepository = userRoomRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid roomId, List<Guid> users, bool invite = false)
        {
            if (roomId == Guid.Empty || users.Count <= 0 || users.Any(x => x == Guid.Empty) ||
                // Check for existing userRoom
                userRoomRepository.FindBy(x => x.Room.Id == roomId && users.Any(y => y == x.User.Id)).Count() > 0)
            {
                return false;
            }

            var room = roomRepository.GetById(roomId);

            if (room == null || !room.IsPublic && !invite)
            {
                return false;
            }

            foreach (var user in users)
            {
                var userFromDb = userRepository.GetById(user);

                if (userFromDb == null)
                {
                    return false;
                }

                var userRoomToAdd = new DataAccess.Models.UserRoom { User = userFromDb, Room = room };

                userRoomRepository.Add(userRoomToAdd);
            }

            _unitOfWork.Save();

            return true;
        }
    }
}