using System;
using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUserPrivateConversations : IGetUserPrivateConversations

    {
        private readonly IUserRepository userRepository;

        public GetUserPrivateConversations(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public List<UserRoomModel> Invoke(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            var dbUserRoom = userRepository.GetPrivateConversations(userId);

            if (dbUserRoom == null || dbUserRoom.Count() <= 0)
            {
                return null;
            }

            var userRoomList = new List<UserRoomModel>();

            foreach (var item in dbUserRoom)
            {
                var newItem = AutoMapper.Mapper.Map<UserRoomModel>(item);
                userRoomList.Add(newItem);
            }

            return userRoomList;
        }
    }
}