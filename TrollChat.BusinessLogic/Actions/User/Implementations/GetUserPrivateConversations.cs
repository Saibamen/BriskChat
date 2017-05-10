using System;
using System.Collections.Generic;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
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

            if (dbUserRoom == null)
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