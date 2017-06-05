using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class EditRoomDescription : IEditRoomDescription
    {
        private readonly IRoomRepository roomRepository;

        public EditRoomDescription(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public bool Invoke(Guid roomId, string roomDescription)
        {
            if (roomId == Guid.Empty)
            {
                return false;
            }

            var roomToEdit = roomRepository.GetById(roomId);

            switch (roomToEdit)
            {
                default:
                    roomToEdit.Description = roomDescription;
                    roomRepository.Edit(roomToEdit);
                    roomRepository.Save();
                    return true;

                case null:
                    return false;
            }
        }
    }
}