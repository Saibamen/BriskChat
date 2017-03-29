using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;

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

                var token = addNewUser.Invoke(model).Tokens.FirstOrDefault().SecretToken;
                confirmUserEmail.Invoke(token);
            }
        }
    }
}