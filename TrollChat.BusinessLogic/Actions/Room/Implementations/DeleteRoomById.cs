using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class DeleteRoomById : IDeleteRoomById
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoomById(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
        {
            this.roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
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
            _unitOfWork.Save();


            return true;
        }
    }
}