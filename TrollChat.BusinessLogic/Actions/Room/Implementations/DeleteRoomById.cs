using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class DeleteRoomById : IDeleteRoomById
    {
        private readonly IRoomRepository roomRepository;

        public DeleteRoomById(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public bool Invoke(Guid roomId)
        {
            if (roomId == Guid.Empty)
            {
                return false;
            }

            var roomToDelete = roomRepository.GetById(roomId);

            // Add test for name
            if (roomToDelete == null || roomToDelete.Name == "general")
            {
                return false;
            }

            roomRepository.Delete(roomToDelete);
            roomRepository.Save();

            return true;
        }
    }
}