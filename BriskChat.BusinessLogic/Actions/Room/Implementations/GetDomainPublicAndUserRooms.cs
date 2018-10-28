using System;
using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
{
    public class GetDomainPublicAndUserRooms : IGetDomainPublicAndUserRooms
    {
        private readonly IRoomRepository roomRepository;

        public GetDomainPublicAndUserRooms(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public List<RoomModel> Invoke(Guid domainId, Guid userId)
        {
            if (domainId == Guid.Empty || userId == Guid.Empty)
            {
                return null;
            }

            var dbRooms = roomRepository.GetDomainPublicAndUserRooms(domainId, userId).ToList();

            if (dbRooms == null)
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
                CreatedOn = item.CreatedOn
            }).GroupBy(x => x.Id).Select(group => group.First()).ToList();

            return returnList;
        }
    }
}