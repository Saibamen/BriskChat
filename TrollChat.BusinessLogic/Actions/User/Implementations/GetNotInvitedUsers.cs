﻿using System;
using System.Collections.Generic;
using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
{
    public class GetNotInvitedUsers : IGetNotInvitedUsers
    {
        private readonly IUserRepository userRepository;

        public GetNotInvitedUsers(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public List<UserModel> Invoke(Guid domainId, Guid roomId)
        {
            if (domainId == Guid.Empty || roomId == Guid.Empty)
            {
                return null;
            }

            var result = userRepository.FindBy(x => x.Domain.Id == domainId).Where(x => x.UserRooms.All(y => y.Room.Id != roomId));

            var userList = new List<UserModel>();

            foreach (var item in result)
            {
                userList.Add(new UserModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email
                });
            }

            return userList;
        }
    }
}