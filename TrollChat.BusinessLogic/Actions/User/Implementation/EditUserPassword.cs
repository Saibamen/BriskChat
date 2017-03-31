using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Helpers.Implementations;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementation
{
    public class EditUserPassword : IEditUserPassword
    {
        private readonly IUserTokenRepository userTokenRepository;
        private readonly IUserRepository userRepository;
        private readonly IHasher hasher;

        public EditUserPassword(
            IUserTokenRepository userTokenRepository,
            IUserRepository userRepository,
            IHasher hasher = null)
        {
            this.userTokenRepository = userTokenRepository;
            this.userRepository = userRepository;
            this.hasher = hasher ?? new Hasher();
        }

        public bool Invoke(int id, string plainPassword)
        {
            if (string.IsNullOrEmpty(plainPassword))
            {
                return false;
            }

            var userToEdit = userRepository.GetById(id);

            if (userToEdit == null)
            {
                return false;
            }

            var salt = hasher.GenerateRandomSalt();
            userToEdit.PasswordHash = hasher.CreatePasswordHash(plainPassword, salt);
            userToEdit.PasswordSalt = salt;

            userRepository.Edit(userToEdit);
            userRepository.Save();

            var tokenToDelete = userToEdit.Tokens.FirstOrDefault();
            if (tokenToDelete != null)
            {
                userTokenRepository.Delete(tokenToDelete);
                userTokenRepository.Save();
            }

            return true;
        }
    }
}