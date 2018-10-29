using System;
using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
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

            if (dbRoom == null || dbRoom.Count() <= 0)
            {
                return null;
            }

            var returnList = dbRoom.ToList().Select(item => new UserModel
            {
                Id = item.Id,
                Name = item.Name,
                Email = item.Email
            }).ToList();

            return returnList;
        }
    }
}