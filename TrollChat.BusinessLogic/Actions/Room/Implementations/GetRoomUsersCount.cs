using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class GetRoomUsersCount : IGetRoomUsersCount
    {
        private readonly IRoomRepository roomRepository;

        public GetRoomUsersCount(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public int Invoke(Guid roomId)
        {
            if (roomId == Guid.Empty)
            {
                return 0;
            }

            var usersCount = roomRepository.GetRoomUsersCount(roomId);

            return usersCount;
        }
    }
}