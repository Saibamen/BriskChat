using System;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
{
    public class GetRoomInformation : IGetRoomInformation
    {
        private readonly IRoomRepository roomRepository;

        public GetRoomInformation(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public RoomModel Invoke(Guid roomId)
        {
            if (roomId == Guid.Empty)
            {
                return null;
            }

            var dbRoom = roomRepository.GetRoomInformation(roomId).FirstOrDefault();

            if (dbRoom == null)
            {
                return null;
            }

            var room = new RoomModel
            {
                Name = dbRoom.Name,
                Owner = new UserModel
                {
                    Name = dbRoom.Owner.Name
                },
                Description = dbRoom.Description,
                Topic = dbRoom.Topic,
                Customization = dbRoom.Customization,
                CreatedOn = dbRoom.CreatedOn,
                IsPrivateConversation = dbRoom.IsPrivateConversation,
                IsPublic = dbRoom.IsPublic
            };

            return room;
        }
    }
}