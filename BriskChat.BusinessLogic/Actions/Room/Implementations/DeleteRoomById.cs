using System;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
{
    public class DeleteRoomById : IDeleteRoomById
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoomById(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
        {
            _roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid roomId)
        {
            if (roomId == Guid.Empty)
            {
                return false;
            }

            var roomToDelete = _roomRepository.GetById(roomId);

            // Add test for name
            if (roomToDelete == null || roomToDelete.Name == "general")
            {
                return false;
            }

            _roomRepository.Delete(roomToDelete);
            _unitOfWork.Save();

            return true;
        }
    }
}