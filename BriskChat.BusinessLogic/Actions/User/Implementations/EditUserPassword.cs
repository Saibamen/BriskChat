using System;
using System.Linq;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Helpers.Implementations;
using BriskChat.BusinessLogic.Helpers.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
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

                    userRepository.Edit(userToEdit);
                    _unitOfWork.Save();

                    return true;

                case null:
                    return false;
            }
        }
    }
}