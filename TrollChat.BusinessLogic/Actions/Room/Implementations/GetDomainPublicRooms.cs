using System;
using System.Collections.Generic;
using System.Linq;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Room.Implementations
{
    public class GetDomainPublicRooms : IGetDomainPublicRooms
    {
        private readonly IRoomRepository roomRepository;

        public GetDomainPublicRooms(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public List<RoomModel> Invoke(Guid domainId)
        {
            if (domainId == Guid.Empty)
            {
                return null;
            }

            var dbRooms = roomRepository.FindBy(x => x.Domain.Id == domainId && x.IsPublic).ToList();

            if (dbRooms == null)
            {
                return null;
            }

            var returnList = dbRooms.Select(item => new RoomModel
            {
                Id = item.Id,
                Name = item.Name,
                IsPublic = item.IsPublic,
                IsPrivateConversation = item.IsPrivateConversation
            }).ToList();

            return returnList;
        }
    }
}