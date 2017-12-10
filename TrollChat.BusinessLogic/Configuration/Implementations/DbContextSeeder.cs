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
        private readonly IAddNewUser addNewUser;
        private readonly IConfirmUserEmailByToken confirmUserEmailByToken;
        private readonly IAddNewDomain addNewDomain;
        private readonly IGetDomainByName getDomainByName;
        private readonly ISetDomainOwner setDomainOwner;
        private readonly IAddNewRole addNewRole;
        private readonly IGetRoleByName getRoleByName;
        private readonly IAddUserToDomain addUserToDomain;

        public DbContextSeeder(IAddNewUser addNewUser,
            IConfirmUserEmailByToken confirmUserEmailByToken,
            IAddNewDomain addNewDomain,
            IGetDomainByName getDomainByName,
            ISetDomainOwner setDomainOwner,
            IAddNewRole addNewRole,
            IGetRoleByName getRoleByName,
            IAddUserToDomain addUserToDomain)
        {
            this.addNewUser = addNewUser;
            this.addNewDomain = addNewDomain;
            this.confirmUserEmailByToken = confirmUserEmailByToken;
            this.getDomainByName = getDomainByName;
            this.setDomainOwner = setDomainOwner;
            this.addNewRole = addNewRole;
            this.getRoleByName = getRoleByName;
            this.addUserToDomain = addUserToDomain;
        }

        public bool Seed()
        {
            var rolesIsSeeded = SeedRoles(addNewRole);

            if (!rolesIsSeeded)
            {
                return false;
            }

            SeedDomains(addNewDomain);
            SeedUsers(addNewUser, confirmUserEmailByToken, getDomainByName, setDomainOwner);

            return true;
        }

        private readonly List<RoleModel> roles = new List<RoleModel>
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

        private readonly string[] users = { "owner", "user", "user1" };
        private readonly string[] domains = { "jan", "roland" };

        public bool SeedRoles(IAddNewRole addNewRole)
        {
            var rolesInDb = 0;

            foreach (var role in roles)
            {
                var roleInDb = getRoleByName.Invoke(role.Name);

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

            return roles.Count == rolesInDb;
        }

        public void SeedDomains(IAddNewDomain addNewDomain)
        {
            foreach (var domain in domains)
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
            foreach (var user in users)
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

                var role = roles.Find(x => x.Name == "User");

                if (user != "owner")
                {
                    setDomainOwner.Invoke(dbUser.Id, domain.Id);
                    var domain2 = getDomainByName.Invoke("roland");
                    setDomainOwner.Invoke(dbUser.Id, domain2.Id);

                    role = roles.Find(x => x.Name == "Owner");
                }

                addUserToDomain.Invoke(dbUser.Id, domain.Id, role.Id);
            }
        }
    }
}