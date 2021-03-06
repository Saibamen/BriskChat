﻿using System;
using BriskChat.BusinessLogic.Actions.Role.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Role.Implementations
{
    public class AddNewRole : IAddNewRole
    {
        private readonly IRoleRepository roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddNewRole(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            this.roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public Guid Invoke(RoleModel model)
        {
            if (!model.IsValid())
            {
                return Guid.Empty;
            }

            var dbRole = AutoMapper.Mapper.Map<DataAccess.Models.Role>(model);

            roleRepository.Add(dbRole);
            _unitOfWork.Save();


            return dbRole.Id;
        }
    }
}