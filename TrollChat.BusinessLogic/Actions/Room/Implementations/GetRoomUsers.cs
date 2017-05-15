using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class GetRoomUsers : IGetRoomUsers
    {
        private readonly IRoomRepository roomRepository;

        public GetRoomUsers(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public List<UserModel> Invoke(Guid roomId)
        {
            if (roomId == Guid.Empty)
            {
                return null;
            }

            var dbRoom = roomRepository.GetRoomUsers(roomId);

            if (dbRoom == null)
            {
                return null;
            }

            var returnList = dbRoom.ToList().Select(item => new UserModel
            {
                Id = item.Id,
                Name = item.Name,
                Email = item.Email,
            }).ToList();

            return returnList;
        }
    }
}