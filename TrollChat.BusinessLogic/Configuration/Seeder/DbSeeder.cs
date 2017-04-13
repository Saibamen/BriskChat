using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Context;
using System;

namespace TrollChat.BusinessLogic.Configuration.Seeder
{
    public class DbContextSeeder
    {
        private readonly TrollChatDbContext context;

        public DbContextSeeder(TrollChatDbContext context)
        {
            this.context = context;
        }

        public void Seed(IAddNewUser addNewUser, IConfirmUserEmailByToken confirmUserEmailByToken)
        {
            SeedUsers(addNewUser, confirmUserEmailByToken);
        }

        private readonly string[] users = { "owner", "user" };

        public void SeedUsers(IAddNewUser addNewUser, IConfirmUserEmailByToken confirmUserEmailByToken)
        {
            foreach (var user in users)
            {
                var model = new UserModel
                {
                    Email = $"{user}@test.com",
                    Password = "test",
                    Name = user
                };

                var token = addNewUser.Invoke(model).Tokens.FirstOrDefault().SecretToken;
                confirmUserEmailByToken.Invoke(token);
            }
        }
    }
}