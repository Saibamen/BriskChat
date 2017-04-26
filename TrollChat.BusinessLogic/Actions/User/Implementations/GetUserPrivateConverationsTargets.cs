using System;
using System.Collections.Generic;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUserPrivateConversationsTargets : IGetUserPrivateConversationsTargets
    {
        private readonly IUserRepository userRepository;

        public GetUserPrivateConversationsTargets(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public List<UserRoomModel> Invoke(Guid userId)
        {
            var dbUser = userRepository.GetPrivateConversationsTargets(userId);

            if (dbUser == null)
            {
                return null;
            }

            var user = AutoMapper.Mapper.Map<List<UserRoomModel>>(dbUser);

            return user;
        }
    }
}