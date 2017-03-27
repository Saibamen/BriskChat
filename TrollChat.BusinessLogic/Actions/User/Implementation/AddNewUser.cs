using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementation
{
    public class AddNewUser : IAddNewUser
    {
        private readonly IUserRepository userRepository;

        public AddNewUser(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public int Invoke(Models.User user)
        {
            // TODO: !user.IsValid()
            if (userRepository.FindBy(x => x.Email == user.Email).Any())
            {
                return 0;
            }

            // FIXME
            var newUser = new DataAccess.Models.User
            {
                Email = user.Email,
                Name = user.Name
            };

            userRepository.Add(newUser);
            userRepository.Save();

            return newUser.Id;
        }
    }
}