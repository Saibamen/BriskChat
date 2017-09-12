using System;
using TrollChat.BusinessLogic.Actions.Role.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Role.Implementations
{
    public class AddNewRole : IAddNewRole
    {
        private readonly IRoleRepository roleRepository;

        public AddNewRole(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }

        public Guid Invoke(RoleModel model)
        {
            if (!model.IsValid())
            {
                return Guid.Empty;
            }

            var dbRole = AutoMapper.Mapper.Map<DataAccess.Models.Role>(model);

            roleRepository.Add(dbRole);
            roleRepository.Save();

            return dbRole.Id;
        }
    }
}