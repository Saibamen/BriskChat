﻿using System;
using System.Linq;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Domain.Implementations
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

        public Guid Invoke(DomainModel domain, Guid? userId = null)
        {
            if (!domain.IsValid() || domainRepository.FindBy(x => x.Name == domain.Name).Any())
            {
                return Guid.Empty;
            }

            var newDomain = AutoMapper.Mapper.Map<DataAccess.Models.Domain>(domain);
            domainRepository.Add(newDomain);
            domainRepository.Save();

            return newDomain.Id;
        }
    }
}