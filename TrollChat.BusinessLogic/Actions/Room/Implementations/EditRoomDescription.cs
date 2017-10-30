using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class EditRoomDescription : IEditRoomDescription
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditRoomDescription(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
        {
            this.roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid roomId, string roomDescription)
        {
            if (roomId == Guid.Empty || roomDescription.Length > 100)
            {
                return false;
            }

            var roomToEdit = roomRepository.GetById(roomId);

            switch (roomToEdit)
            {
                default:
                    roomToEdit.Description = roomDescription;
                    roomRepository.Edit(roomToEdit);
                    _unitOfWork.Save();
                    return true;

                case null:
                    return false;
            }
        }
    }
}