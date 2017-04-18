using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Actions.Domain.Interface;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Context;
using System;
using TrollChat.BusinessLogic.Actions.User.Implementation;

namespace TrollChat.BusinessLogic.Configuration.Seeder
{
    public class DbContextSeeder
    {
        private readonly TrollChatDbContext context;

        public DbContextSeeder(TrollChatDbContext context)
        {
            this.context = context;
        }

        public void Seed(IAddNewUser addNewUser, IConfirmUserEmailByToken confirmUserEmailByToken, IAddNewDomain addNewDomain, IGetUserByEmail getUserByEmail)
        {
            SeedUsers(addNewUser, confirmUserEmailByToken);
            SeedDomain(addNewDomain, getUserByEmail);
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

                var dbuser = addNewUser.Invoke(model);
                var token = dbuser.Tokens.FirstOrDefault().SecretToken;
                confirmUserEmailByToken.Invoke(token);
            }
        }

        public void SeedDomain(IAddNewDomain addNewDomain, IGetUserByEmail getUserByEmail)
        {
            foreach (var user in users)
            {
                string email = $"{user}@test.com";

                var model = new DomainModel
                {
                    Name = $"{user}domain",
                    Owner = AutoMapper.Mapper.Map<BusinessLogic.Models.UserModel>(getUserByEmail.Invoke(email)),
                };

                var dbdomain = addNewDomain.Invoke(model);
            }
        }
    }
}