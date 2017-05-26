using System;
using System.Linq;
using TrollChat.BusinessLogic.Actions.UserRoom.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserRoom.Implementations
{
    public class AddNewUserRoom : IAddNewUserRoom
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserRoomRepository userRoomRepository;

        public AddNewUserRoom(IRoomRepository roomRepository,
            IUserRepository userRepository,
            IUserRoomRepository userRoomRepository)
        {
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
            this.userRoomRepository = userRoomRepository;
        }

        public bool Invoke(Guid roomId, Guid userId)
        {
            if (roomId == Guid.Empty || userId == Guid.Empty ||
                // Check for existing userRoom
                userRoomRepository.FindBy(x => x.Room.Id == roomId && x.User.Id == userId).Count() > 0)
            {
                return false;
            }

            var user = userRepository.GetById(userId);

            if (user == null)
            {
                return false;
            }

            var room = roomRepository.GetById(roomId);

            // TODO: Implement inviting to private rooms
            if (room == null || !room.IsPublic)
            {
                return false;
            }

            var userRoom = new DataAccess.Models.UserRoom { User = user, Room = room };

            userRoomRepository.Add(userRoom);
            userRoomRepository.Save();

            return true;
        }
    }
}