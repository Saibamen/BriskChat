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
        private readonly IDomainRepository domainRepository;

        public AddNewPrivateConversation(IRoomRepository roomRepository,
            IUserRepository userRepository,
            IUserRoomRepository userRoomRepository,
            IDomainRepository domainRepository)
        {
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
            this.userRoomRepository = userRoomRepository;
            this.domainRepository = domainRepository;
        }

        public Guid Invoke(RoomModel room, Guid issuerUserId, Guid secondUserId)
        {
            if (!room.IsValid() || issuerUserId == secondUserId)
            {
                return Guid.Empty;
            }

            var domain = domainRepository.GetById(room.Domain.Id);

            if (domain == null)
            {
                return Guid.Empty;
            }

            var privateConversationList = userRepository.GetPrivateConversationsTargets(issuerUserId);

            if (privateConversationList.Any(x => x.User.Id == secondUserId))
            {
                return Guid.Empty;
            }

            var issuerUser = userRepository.GetById(issuerUserId);
            var secondUser = userRepository.GetById(secondUserId);

            if (issuerUser == null || secondUser == null)
            {
                return Guid.Empty;
            }

            var newRoom = AutoMapper.Mapper.Map<DataAccess.Models.Room>(room);
            newRoom.Owner = AutoMapper.Mapper.Map<DataAccess.Models.User>(issuerUser);
            newRoom.Domain = AutoMapper.Mapper.Map<DataAccess.Models.Domain>(domain);
            newRoom.IsPrivateConversation = true;

            roomRepository.Add(newRoom);
            roomRepository.Save();

            var userRoom = new DataAccess.Models.UserRoom { User = issuerUser, Room = newRoom };
            var userRoom2 = new DataAccess.Models.UserRoom { User = secondUser, Room = newRoom };

            userRoomRepository.Add(userRoom);
            userRoomRepository.Add(userRoom2);
            userRoomRepository.Save();

            return newRoom.Id;
        }
    }
}