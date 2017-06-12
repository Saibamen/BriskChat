using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;
using TrollChat.BusinessLogic.Configuration.Interfaces;

namespace TrollChat.BusinessLogic.Configuration.Implementations
{
    public class DbContextSeeder : IDbContextSeeder
    {
        private readonly IAddNewUser addNewUser;
        private readonly IConfirmUserEmailByToken confirmUserEmailByToken;
        private readonly IAddNewDomain addNewDomain;
        private readonly IGetDomainByName getDomainByName;
        private readonly ISetDomainOwner setDomainOwner;

        public DbContextSeeder(IAddNewUser addNewUser,
            IConfirmUserEmailByToken confirmUserEmailByToken,
            IAddNewDomain addNewDomain,
            IGetDomainByName getDomainByName,
            ISetDomainOwner setDomainOwner)
        {
            this.addNewUser = addNewUser;
            this.addNewDomain = addNewDomain;
            this.confirmUserEmailByToken = confirmUserEmailByToken;
            this.getDomainByName = getDomainByName;
            this.setDomainOwner = setDomainOwner;
        }

        public void Seed()
        {
            SeedDomains(addNewDomain);
            SeedUsers(addNewUser, confirmUserEmailByToken, getDomainByName, setDomainOwner);
        }

        private readonly string[] users = { "owner", "user", "user1" };

        private readonly string[] domains = { "jan", "roland" };

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

                var dbuser = addNewUser.Invoke(model);

                if (dbuser != null)
                {
                    var token = dbuser.Tokens.FirstOrDefault().SecretToken;
                    confirmUserEmailByToken.Invoke(token);

                    if (user == "owner")
                    {
                        setDomainOwner.Invoke(dbuser.Id, domain.Id);
                        var domain2 = getDomainByName.Invoke("roland");
                        setDomainOwner.Invoke(dbuser.Id, domain2.Id);
                    }
                }
            }
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
    }
}