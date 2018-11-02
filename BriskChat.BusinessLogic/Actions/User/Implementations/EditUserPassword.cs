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
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHasher _hasher;
        private readonly IUnitOfWork _unitOfWork;

        public EditUserPassword(
            IUserTokenRepository userTokenRepository,
            IUserRepository userRepository, IUnitOfWork unitOfWork, IHasher hasher = null)
        {
            _userTokenRepository = userTokenRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _hasher = hasher ?? new Hasher();
        }

        public bool Invoke(Guid id, string plainPassword)
        {
            if (id == Guid.Empty || string.IsNullOrWhiteSpace(plainPassword))
            {
                return false;
            }

            var userToEdit = _userRepository.GetById(id);

            switch (userToEdit)
            {
                default:
                    var salt = _hasher.GenerateRandomSalt();
                    userToEdit.PasswordHash = _hasher.CreatePasswordHash(plainPassword, salt);
                    userToEdit.PasswordSalt = salt;

                    var tokenToDelete = _userTokenRepository
                        .FindBy(x => x.User == userToEdit)
                        .FirstOrDefault();

                    if (tokenToDelete is null)
                    {
                        return false;
                    }

                    _userTokenRepository.Delete(tokenToDelete);

                    _userRepository.Edit(userToEdit);
                    _unitOfWork.Save();

                    return true;

                case null:
                    return false;
            }
        }
    }
}