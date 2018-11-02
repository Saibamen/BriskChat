using System;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
{
    public class EditRoomDescription : IEditRoomDescription
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditRoomDescription(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
        {
            _roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid roomId, string roomDescription)
        {
            if (roomId == Guid.Empty || roomDescription.Length > 100)
            {
                return false;
            }

            var roomToEdit = _roomRepository.GetById(roomId);

            switch (roomToEdit)
            {
                default:
                    roomToEdit.Description = roomDescription;
                    _roomRepository.Edit(roomToEdit);
                    _unitOfWork.Save();
                    return true;

                case null:
                    return false;
            }
        }
    }
}