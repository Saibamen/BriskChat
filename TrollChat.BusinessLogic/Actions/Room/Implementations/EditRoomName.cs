using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class EditRoomName : IEditRoomName
    {
        private readonly IRoomRepository roomRepository;

        public EditRoomName(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public bool Invoke(Guid roomId, string roomName)
        {
            if (roomId == Guid.Empty)
            {
                return false;
            }

            var roomToEdit = roomRepository.GetById(roomId);

            switch (roomToEdit)
            {
                default:
                    roomToEdit.Name = roomName;
                    roomRepository.Edit(roomToEdit);
                    roomRepository.Save();
                    return true;

                case null:
                    return false;
            }
        }
    }
}