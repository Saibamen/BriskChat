using System;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUserRooms : IGetUserRooms
    {
        private readonly IUserRepository userRepository;

        public GetUserRooms(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public List<RoomModel> Invoke(Guid userId, bool isPrivateConversation = false)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            var dbUser = userRepository.GetUserRooms(userId, isPrivateConversation);

            if (dbUser == null || dbUser.Count() <= 0)
            {
                return null;
            }

            var returnList = dbUser.ToList().Select(item => new RoomModel
            {
                Id = item.Id,
                Name = item.Name,
                IsPublic = item.IsPublic,
                IsPrivateConversation = item.IsPrivateConversation
            }).ToList();

            return returnList;
        }
    }
}