using System;
using System.Linq;
using AutoMapper.QueryableExtensions;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
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
                    Name = dbRoom.Owner.Name,
                    Email = dbRoom.Owner.Email,
                },
                Description = dbRoom.Description,
                Customization = dbRoom.Customization,
                Topic = dbRoom.Topic,
                IsPublic = dbRoom.IsPublic,
                IsPrivateConversation = dbRoom.IsPrivateConversation,
                CreatedOn = dbRoom.CreatedOn,
            };

            return room;
        }
    }
}