using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class EditRoomTopic : IEditRoomTopic
    {
        private readonly IRoomRepository roomRepository;

        public EditRoomTopic(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
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
                    roomRepository.Save();
                    return true;

                case null:
                    return false;
            }
        }
    }
}