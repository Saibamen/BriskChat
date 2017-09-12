using System;
using System.Linq;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class GetRoomByName : IGetRoomByName
    {
        private readonly IRoomRepository roomRepository;

        public GetRoomByName(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public RoomModel Invoke(string roomName, Guid domainId)
        {
            if (string.IsNullOrEmpty(roomName) || domainId == Guid.Empty)
            {
                return null;
            }

            var dbRoom = roomRepository.FindBy(x => x.Name == roomName && x.Domain.Id == domainId).FirstOrDefault();

            if (dbRoom == null)
            {
                return null;
            }

            var room = AutoMapper.Mapper.Map<RoomModel>(dbRoom);

            return room;
        }
    }
}