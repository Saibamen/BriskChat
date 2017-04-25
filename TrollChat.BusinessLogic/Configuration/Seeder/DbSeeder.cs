using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;

namespace TrollChat.BusinessLogic.Configuration.Seeder
{
    public class DbContextSeeder
    {
        public void Seed(IAddNewUser addNewUser,
            IConfirmUserEmailByToken confirmUserEmailByToken,
            IAddNewDomain addNewDomain,
            IGetUserByEmail getUserByEmail,
            IGetDomainByName getDomainByName)
        {
            SeedDomain(addNewDomain, getUserByEmail);
            SeedUsers(addNewUser, confirmUserEmailByToken, getDomainByName);
        }

        private readonly string[] users = { "owner", "user" };

        public void SeedUsers(IAddNewUser addNewUser, IConfirmUserEmailByToken confirmUserEmailByToken, IGetDomainByName getDomainByName)
        {
            foreach (var user in users)
            {
                var model = new UserModel
                {
                    Email = $"{user}@test.com",
                    Password = "test",
                    Name = user,
                    Domain = getDomainByName.Invoke("jan")
                };

                var dbuser = addNewUser.Invoke(model);
                var token = dbuser.Tokens.FirstOrDefault().SecretToken;
                confirmUserEmailByToken.Invoke(token);
            }
        }

        public void SeedDomain(IAddNewDomain addNewDomain, IGetUserByEmail getUserByEmail)
        {
            foreach (var user in users)
            {
                var model = new DomainModel
                {
                    Name = "jan"
                };

                addNewDomain.Invoke(model);
            }
        }
    }
}