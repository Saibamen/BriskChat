using System;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
{
    public class GetRoomByName : IGetRoomByName
    {
        private readonly IRoomRepository _roomRepository;

        public GetRoomByName(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public RoomModel Invoke(string roomName, Guid domainId)
        {
            if (string.IsNullOrWhiteSpace(roomName) || domainId == Guid.Empty)
            {
                return null;
            }

            var dbRoom = _roomRepository
                .FindBy(x => x.Name == roomName && x.Domain.Id == domainId)
                .FirstOrDefault();

            if (dbRoom == null)
            {
                return null;
            }

            var room = AutoMapper.Mapper.Map<RoomModel>(dbRoom);

            return room;
        }
    }
}