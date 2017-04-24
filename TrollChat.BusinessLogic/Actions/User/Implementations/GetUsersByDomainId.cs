using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUsersByDomainId : IGetUsersByDomainId
    {
        private readonly IUserRepository userRepository;

        public GetUsersByDomainId(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public List<UserModel> Invoke(Guid domainId)
        {
            if (domainId == Guid.Empty)
            {
                return null;
            }

            var result = userRepository.FindBy(x => x.Domain.Id == domainId);

            var userList = new List<UserModel>();

            foreach (var item in result)
            {
                userList.Add(new UserModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                });
            }

            return userList;
        }
    }
}