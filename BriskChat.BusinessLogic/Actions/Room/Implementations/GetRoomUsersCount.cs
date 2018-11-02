using System;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
{
    public class GetRoomUsersCount : IGetRoomUsersCount
    {
        private readonly IRoomRepository _roomRepository;

        public GetRoomUsersCount(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public int Invoke(Guid roomId)
        {
            if (roomId == Guid.Empty)
            {
                return 0;
            }

            var usersCount = _roomRepository.GetRoomUsersCount(roomId);

            return usersCount;
        }
    }
}