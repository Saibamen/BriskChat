using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using System.Linq;

namespace TrollChat.BusinessLogic.Actions.Room.Implementation
{
    public class AddNewRoom : IAddNewRoom
    {
        private IRoomRepository roomRepository;

        public AddNewRoom(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public int Invoke(Models.Room room)
        {
            if (!room.IsValid())
            {
                return 0;
            }

            var newRoom = new DataAccess.Models.Room
            {
                Name = room.Name,
                Topic = room.Topic,
                Description = room.Description,
                Customization = room.Customization,
                IsPublic = true,
            };

            roomRepository.Add(newRoom);
            roomRepository.Save();
            return newRoom.Id;
        }
    }
}