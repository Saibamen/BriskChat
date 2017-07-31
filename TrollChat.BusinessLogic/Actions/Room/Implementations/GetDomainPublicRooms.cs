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

            if (dbRooms == null || dbRooms.Count <= 0)
            {
                return null;
            }

            var returnList = dbRooms.Select(item => new RoomModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                IsPublic = item.IsPublic,
                Owner = new UserModel
                {
                    Name = item.Owner.Name
                },
                IsPrivateConversation = item.IsPrivateConversation,
                CreatedOn = item.CreatedOn
            }).ToList();

            return returnList;
        }
    }
}