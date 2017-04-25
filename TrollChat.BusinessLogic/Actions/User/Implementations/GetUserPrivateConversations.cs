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
            var dbUser = userRepository.GetPrivateConversations(userId);

            if (dbUser == null)
            {
                return null;
            }

            var user = AutoMapper.Mapper.Map<List<UserRoomModel>>(dbUser);

            return user;
        }
    }
}