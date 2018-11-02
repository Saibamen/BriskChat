using System;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
{
    public class EditRoomName : IEditRoomName
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditRoomName(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
        {
            _roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid roomId, string roomName)
        {
            if (roomId == Guid.Empty || string.IsNullOrWhiteSpace(roomName) || roomName.Length > 100)
            {
                return false;
            }

            var roomToEdit = _roomRepository.GetById(roomId);

            if (roomToEdit == null || roomToEdit.IsPrivateConversation)
            {
                return false;
            }

            roomToEdit.Name = roomName;
            _roomRepository.Edit(roomToEdit);
            _unitOfWork.Save();

            return true;
        }
    }
}