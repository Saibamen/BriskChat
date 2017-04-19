﻿using TrollChat.BusinessLogic.Actions.Domain.Interface;
using TrollChat.DataAccess.Repositories.Interfaces;
using System.Linq;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.BusinessLogic.Actions.Domain.Implementation
{
    public class AddNewDomain : IAddNewDomain
    {
        private readonly IDomainRepository domainRepository;
        private readonly IUserRepository userRepository;

        public AddNewDomain(IDomainRepository domainRepository, IUserRepository userRepository)
        {
            this.domainRepository = domainRepository;
            this.userRepository = userRepository;
        }

        public int Invoke(DomainModel domain, int userId)
        {
            if (!domain.IsValid() || domainRepository.FindBy(x => x.Name == domain.Name).Any())
            {
                return 0;
            }

            var user = userRepository.GetById(userId);

            if (user == null)
            {
                return 0;
            }

            var newDomain = AutoMapper.Mapper.Map<DataAccess.Models.Domain>(domain);
            newDomain.Owner = AutoMapper.Mapper.Map<DataAccess.Models.User>(user);
            domainRepository.Add(newDomain);
            domainRepository.Save();

            return newDomain.Id;
        }
    }
}