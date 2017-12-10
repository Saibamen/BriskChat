using System;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
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