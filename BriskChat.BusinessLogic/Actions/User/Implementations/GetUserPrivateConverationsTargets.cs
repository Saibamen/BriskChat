using System;
using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUserPrivateConversationsTargets : IGetUserPrivateConversationsTargets
    {
        private readonly IUserRepository _userRepository;

        public GetUserPrivateConversationsTargets(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<UserRoomModel> Invoke(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            var dbUserRooms = _userRepository.GetPrivateConversationsTargets(userId);

            if (dbUserRooms == null || dbUserRooms.Count() <= 0)
            {
                return null;
            }

            var user = AutoMapper.Mapper.Map<List<UserRoomModel>>(dbUserRooms);

            return user;
        }
    }
}