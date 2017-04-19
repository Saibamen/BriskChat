using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementation
{
    public class DeleteRoomById : IDeleteRoomById
    {
        private readonly IRoomRepository roomRepository;

        public DeleteRoomById(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public bool Invoke(int roomId)
        {
            var roomToDelete = roomRepository.GetById(roomId);

            if (roomToDelete == null)
            {
                return false;
            }

            roomRepository.Delete(roomToDelete);
            roomRepository.Save();

            return true;
        }
    }
}