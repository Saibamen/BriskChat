using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementation
{
    public class AddNewRoom : IAddNewRoom
    {
        private readonly IRoomRepository roomRepository;

        public AddNewRoom(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public int Invoke(Models.RoomModel room)
        {
            if (!room.IsValid())
            {
                return 0;
            }

            var newRoom = AutoMapper.Mapper.Map<DataAccess.Models.Room>(room);
            newRoom.IsPublic = true;

            roomRepository.Add(newRoom);
            roomRepository.Save();

            return newRoom.Id;
        }
    }
}