using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class EditRoomCustomization : IEditRoomCustomization
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditRoomCustomization(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
        {
            this.roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid roomId, int roomCustomization)
        {
            if (roomId == Guid.Empty || roomCustomization < 0)
            {
                return false;
            }

            var roomToEdit = roomRepository.GetById(roomId);

            switch (roomToEdit)
            {
                default:
                    roomToEdit.Customization = roomCustomization;
                    roomRepository.Edit(roomToEdit);
                    _unitOfWork.Save();
                    return true;

                case null:
                    return false;
            }
        }
    }
}