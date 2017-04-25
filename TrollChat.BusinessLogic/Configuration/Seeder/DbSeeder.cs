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
            IGetDomainByName getDomainByName)
        {
            SeedDomain(addNewDomain);
            SeedUsers(addNewUser, confirmUserEmailByToken, getDomainByName);
        }

        private readonly string[] users = { "owner", "user", "user1" };

        public void SeedUsers(IAddNewUser addNewUser,
            IConfirmUserEmailByToken confirmUserEmailByToken,
            IGetDomainByName getDomainByName)
        {
            foreach (var user in users)
            {
                var model = new UserModel
                {
                    Email = $"{user}@test.pl",
                    Password = "test",
                    Name = user,
                    Domain = getDomainByName.Invoke("jan")
                };

                var dbuser = addNewUser.Invoke(model);
                var token = dbuser.Tokens.FirstOrDefault().SecretToken;
                confirmUserEmailByToken.Invoke(token);
            }
        }

        public void SeedDomain(IAddNewDomain addNewDomain)
        {
            var model = new DomainModel
            {
                Name = "jan"
            };

            addNewDomain.Invoke(model);
        }
    }
}