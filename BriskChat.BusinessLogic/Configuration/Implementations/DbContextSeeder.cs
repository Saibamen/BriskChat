using System;
using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Domain.Interfaces;
using BriskChat.BusinessLogic.Actions.Role.Interfaces;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Actions.UserDomain.Interfaces;
using BriskChat.BusinessLogic.Configuration.Interfaces;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Configuration.Implementations
{
    public class DbContextSeeder : IDbContextSeeder
    {
        private readonly IAddNewUser _addNewUser;
        private readonly IConfirmUserEmailByToken _confirmUserEmailByToken;
        private readonly IAddNewDomain _addNewDomain;
        private readonly IGetDomainByName _getDomainByName;
        private readonly ISetDomainOwner _setDomainOwner;
        private readonly IAddNewRole _addNewRole;
        private readonly IGetRoleByName _getRoleByName;
        private readonly IAddUserToDomain _addUserToDomain;

        public DbContextSeeder(IAddNewUser addNewUser,
            IConfirmUserEmailByToken confirmUserEmailByToken,
            IAddNewDomain addNewDomain,
            IGetDomainByName getDomainByName,
            ISetDomainOwner setDomainOwner,
            IAddNewRole addNewRole,
            IGetRoleByName getRoleByName,
            IAddUserToDomain addUserToDomain)
        {
            _addNewUser = addNewUser;
            _addNewDomain = addNewDomain;
            _confirmUserEmailByToken = confirmUserEmailByToken;
            _getDomainByName = getDomainByName;
            _setDomainOwner = setDomainOwner;
            _addNewRole = addNewRole;
            _getRoleByName = getRoleByName;
            _addUserToDomain = addUserToDomain;
        }

        public bool Seed()
        {
            var rolesIsSeeded = SeedRoles(_addNewRole);

            if (!rolesIsSeeded)
            {
                return false;
            }

            SeedDomains(_addNewDomain);
            SeedUsers(_addNewUser, _confirmUserEmailByToken, _getDomainByName, _setDomainOwner);

            return true;
        }

        private readonly List<RoleModel> _roles = new List<RoleModel>
        {
            new RoleModel
            {
                Name = "Owner",
                Description = "Creator of Domain or Room."
            },
            new RoleModel
            {
                Name = "Admin",
                Description = "Administrator"
            },
            new RoleModel
            {
                Name = "User",
                Description = "User"
            }
        };

        private readonly string[] _users = { "owner", "user", "user1" };
        private readonly string[] _domains = { "jan", "roland" };

        public bool SeedRoles(IAddNewRole addNewRole)
        {
            var rolesInDb = 0;

            foreach (var role in _roles)
            {
                var roleInDb = _getRoleByName.Invoke(role.Name);

                if (roleInDb == null || roleInDb.Id == Guid.Empty)
                {
                    var roleId = addNewRole.Invoke(role);

                    if (roleId == Guid.Empty)
                    {
                        continue;
                    }

                    role.Id = roleId;
                    rolesInDb++;
                }
                else
                {
                    role.Id = roleInDb.Id;
                    rolesInDb++;
                }
            }

            return _roles.Count == rolesInDb;
        }

        public void SeedDomains(IAddNewDomain addNewDomain)
        {
            foreach (var domain in _domains)
            {
                var model = new DomainModel
                {
                    Name = domain
                };

                addNewDomain.Invoke(model);
            }
        }

        public void SeedUsers(IAddNewUser addNewUser,
            IConfirmUserEmailByToken confirmUserEmailByToken,
            IGetDomainByName getDomainByName,
            ISetDomainOwner setDomainOwner)
        {
            foreach (var user in _users)
            {
                var domain = getDomainByName.Invoke("jan");
                var model = new UserModel
                {
                    Email = $"{user}@test.pl",
                    Password = "test",
                    Name = user,
                    Domain = domain
                };

                var dbUser = addNewUser.Invoke(model);

                if (dbUser == null)
                {
                    continue;
                }

                var token = dbUser.Tokens.FirstOrDefault().SecretToken;
                confirmUserEmailByToken.Invoke(token);

                var role = _roles.Find(x => x.Name == "User");

                if (user != "owner")
                {
                    setDomainOwner.Invoke(dbUser.Id, domain.Id);
                    var domain2 = getDomainByName.Invoke("roland");
                    setDomainOwner.Invoke(dbUser.Id, domain2.Id);

                    role = _roles.Find(x => x.Name == "Owner");
                }

                _addUserToDomain.Invoke(dbUser.Id, domain.Id, role.Id);
            }
        }
    }
}
