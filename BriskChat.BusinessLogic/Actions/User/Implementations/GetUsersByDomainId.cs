using System;
using System.Collections.Generic;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUsersByDomainId : IGetUsersByDomainId
    {
        private readonly IUserRepository _userRepository;

        public GetUsersByDomainId(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<UserModel> Invoke(Guid domainId)
        {
            if (domainId == Guid.Empty)
            {
                return null;
            }

            var result = _userRepository.FindBy(x => x.Domain.Id == domainId);

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