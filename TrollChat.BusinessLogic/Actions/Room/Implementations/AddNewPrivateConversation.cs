using System;
using System.Linq;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class AddNewPrivateConversation : IAddNewPrivateConversation
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserRoomRepository userRoomRepository;

        public AddNewPrivateConversation(IRoomRepository roomRepository, IUserRepository userRepository, IUserRoomRepository userRoomRepository)
        {
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
            this.userRoomRepository = userRoomRepository;
        }

        public Guid Invoke(RoomModel room, Guid userId1, Guid userId2)
        {
            if (!room.IsValid() || userId1 == userId2)
            {
                return Guid.Empty;
            }

            var privateConversationList = userRepository.GetPrivateConversations(userId1);

            if (privateConversationList.Any(x => x.User.Id == userId2))
            {
                return Guid.Empty;
            }

            var user1 = userRepository.GetById(userId1);
            var user2 = userRepository.GetById(userId2);

            if (user1 == null || user2 == null)
            {
                return Guid.Empty;
            }

            var newRoom = AutoMapper.Mapper.Map<DataAccess.Models.Room>(room);
            newRoom.Owner = AutoMapper.Mapper.Map<DataAccess.Models.User>(user1);
            newRoom.IsPrivateConversation = true;

            roomRepository.Add(newRoom);
            roomRepository.Save();

            var userRoom = new DataAccess.Models.UserRoom { User = user1, Room = newRoom };
            var userRoom2 = new DataAccess.Models.UserRoom { User = user2, Room = newRoom };

            userRoomRepository.Add(userRoom);
            userRoomRepository.Add(userRoom2);
            userRoomRepository.Save();

            return newRoom.Id;
        }
    }
}