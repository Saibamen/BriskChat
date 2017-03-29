using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Models;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using System.Linq;

namespace TrollChat.BusinessLogic.Configuration.Seeder
{
    public class DbSeeder
    {
        public void Seed(IAddNewUser addNewUser, IConfirmUserEmail confirmUserEmail)
        {
            SeedUsers(addNewUser, confirmUserEmail);
        }

        private readonly string[] users = { "owner", "user" };

        public void SeedUsers(IAddNewUser addNewUser, IConfirmUserEmail confirmUserEmail)
        {
            foreach (var user in users)
            {
                var model = new User
                {
                    Email = $"{user}@test.com",
                    Password = "test",
                    Name = user
                };

                var userGuid = addNewUser.Invoke(model).SecretToken;
                confirmUserEmail.Invoke(userGuid);
            }
        }
    }
}