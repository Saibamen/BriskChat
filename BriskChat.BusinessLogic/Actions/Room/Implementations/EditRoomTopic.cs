using System;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
{
    public class EditRoomTopic : IEditRoomTopic
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditRoomTopic(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
        {
            _roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid roomId, string roomTopic)
        {
            if (roomId == Guid.Empty || roomTopic.Length > 100)
            {
                return false;
            }

            var roomToEdit = _roomRepository.GetById(roomId);

            switch (roomToEdit)
            {
                default:
                    roomToEdit.Topic = roomTopic;
                    _roomRepository.Edit(roomToEdit);
                    _unitOfWork.Save();
                    return true;

                case null:
                    return false;
            }
        }
    }
}