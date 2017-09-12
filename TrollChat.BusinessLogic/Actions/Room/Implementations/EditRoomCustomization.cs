using System;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class EditRoomCustomization : IEditRoomCustomization
    {
        private readonly IRoomRepository roomRepository;

        public EditRoomCustomization(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
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
                    roomRepository.Save();
                    return true;

                case null:
                    return false;
            }
        }
    }
}