using System;
using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Helpers.Implementations;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
{
    public class EditUserPassword : IEditUserPassword
    {
        private readonly IUserTokenRepository userTokenRepository;
        private readonly IUserRepository userRepository;
        private readonly IHasher hasher;
        private readonly IUnitOfWork _unitOfWork;

        public EditUserPassword(
            IUserTokenRepository userTokenRepository,
            IUserRepository userRepository, IUnitOfWork unitOfWork, IHasher hasher = null)
        {
            this.userTokenRepository = userTokenRepository;
            this.userRepository = userRepository;
            _unitOfWork = unitOfWork;
            this.hasher = hasher ?? new Hasher();
        }

        public bool Invoke(Guid id, string plainPassword)
        {
            if (id == Guid.Empty || string.IsNullOrEmpty(plainPassword))
            {
                return false;
            }

            var userToEdit = userRepository.GetById(id);

            switch (userToEdit)
            {
                default:
                    var salt = hasher.GenerateRandomSalt();
                    userToEdit.PasswordHash = hasher.CreatePasswordHash(plainPassword, salt);
                    userToEdit.PasswordSalt = salt;

                    var tokenToDelete = userTokenRepository.FindBy(x => x.User == userToEdit).FirstOrDefault();

                    if (tokenToDelete is null)
                    {
                        return false;
                    }

                    userTokenRepository.Delete(tokenToDelete);
                    _unitOfWork.Save();

                    userRepository.Edit(userToEdit);
                    _unitOfWork.Save();

                    return true;

                case null:
                    return false;
            }
        }
    }
}