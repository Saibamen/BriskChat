using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class GetRoomById : IGetRoomById
    {
        private readonly IRoomRepository roomRepository;

        public GetRoomById(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public RoomModel Invoke(Guid roomId)
        {
            if (roomId == Guid.Empty)
            {
                return null;
            }

            var dbRoom = roomRepository.GetById(roomId);

            if (dbRoom == null)
            {
                return null;
            }

            var room = AutoMapper.Mapper.Map<RoomModel>(dbRoom);

            return room;
        }
    }
}