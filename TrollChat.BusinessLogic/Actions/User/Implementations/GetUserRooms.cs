using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace TrollChat.BusinessLogic.Actions.User.Implementation
{
    public class GetUserRooms : IGetUserRooms
    {
        private readonly IUserRepository userRepository;

        public GetUserRooms(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public List<RoomModel> Invoke(int userId)
        {
            var dbUser = userRepository.GetUserAndUserRoomsByUserId(userId).FirstOrDefault();

            if (dbUser == null)
            {
                return null;
            }

            var tempList = dbUser.UserRooms.Select(item => new RoomModel()
            {
                Id = item.Room.Id,
                Name = item.Room.Name,
                IsPublic = item.Room.IsPublic,
            }).ToList();

            return tempList;
        }
    }
}