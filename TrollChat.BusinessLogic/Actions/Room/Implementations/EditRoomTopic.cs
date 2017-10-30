using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class EditRoomTopic : IEditRoomTopic
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditRoomTopic(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
        {
            this.roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid roomId, string roomTopic)
        {
            if (roomId == Guid.Empty || roomTopic.Length > 100)
            {
                return false;
            }

            var roomToEdit = roomRepository.GetById(roomId);

            switch (roomToEdit)
            {
                default:
                    roomToEdit.Topic = roomTopic;
                    roomRepository.Edit(roomToEdit);
                    _unitOfWork.Save();
                    return true;

                case null:
                    return false;
            }
        }
    }
}