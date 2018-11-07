﻿using System;
using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUserRooms : IGetUserRooms
    {
        private readonly IUserRepository _userRepository;

        public GetUserRooms(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<RoomModel> Invoke(Guid userId, bool isPrivateConversation = false)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            var dbRooms = _userRepository.GetUserRooms(userId, isPrivateConversation);

            if (dbRooms == null)
            {
                return null;
            }

            var returnList = dbRooms.ToList().Select(item => new RoomModel
            {
                Id = item.Id,
                Name = item.Name,
                IsPublic = item.IsPublic,
                IsPrivateConversation = item.IsPrivateConversation
            }).OrderBy(x => x.Name)
            .ToList();

            return returnList;
        }
    }
}