using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class AddNewRoom : IAddNewRoom
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;

        public AddNewRoom(IRoomRepository roomRepository, IUserRepository userRepository)
        {
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
        }

        public int Invoke(RoomModel room, int userId)
        {
            if (!room.IsValid())
            {
                return 0;
            }

            var user = userRepository.GetById(userId);

            if (user == null)
            {
                return 0;
            }

            var newRoom = AutoMapper.Mapper.Map<DataAccess.Models.Room>(room);
            newRoom.IsPublic = true;
            newRoom.Owner = AutoMapper.Mapper.Map<DataAccess.Models.User>(user);

            roomRepository.Add(newRoom);
            roomRepository.Save();

            return newRoom.Id;
        }
    }
}