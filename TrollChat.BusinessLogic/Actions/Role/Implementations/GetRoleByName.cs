using System.Linq;
using TrollChat.BusinessLogic.Actions.Role.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Role.Implementations
{
    public class GetRoleByName : IGetRoleByName
    {
        private readonly IRoleRepository roleRepository;

        public GetRoleByName(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }

        public RoleModel Invoke(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var result = roleRepository.FindBy(x => x.Name == name)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description
                }).FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            var roleModel = AutoMapper.Mapper.Map<RoleModel>(result);

            return roleModel;
        }
    }
}