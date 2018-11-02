using System;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
{
    public class EditRoomCustomization : IEditRoomCustomization
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditRoomCustomization(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
        {
            _roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid roomId, int roomCustomization)
        {
            if (roomId == Guid.Empty || roomCustomization < 0)
            {
                return false;
            }

            var roomToEdit = _roomRepository.GetById(roomId);

            switch (roomToEdit)
            {
                default:
                    roomToEdit.Customization = roomCustomization;
                    _roomRepository.Edit(roomToEdit);
                    _unitOfWork.Save();
                    return true;

                case null:
                    return false;
            }
        }
    }
}