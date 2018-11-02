using System;
using BriskChat.BusinessLogic.Actions.UserToken.Interfaces;
using BriskChat.BusinessLogic.Helpers.Implementations;
using BriskChat.BusinessLogic.Helpers.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class AddUserTokenToUser : IAddUserTokenToUser
    {
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHasher _hasher;
        private readonly IUnitOfWork _unitOfWork;

        public AddUserTokenToUser(IUserTokenRepository userTokenRepository,
            IUserRepository userRepository, IUnitOfWork unitOfWork, IHasher hasher = null)
        {
            _userTokenRepository = userTokenRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _hasher = hasher ?? new Hasher();
        }

        public string Invoke(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return string.Empty;
            }

            var user = _userRepository.GetById(userId);

            if (user == null)
            {
                return string.Empty;
            }

            var token = _userTokenRepository.GetById(userId);

            if (token != null)
            {
                _userTokenRepository.Delete(token);
            }

            var userToken = new DataAccess.Models.UserToken
            {
                User = user,
                SecretToken = _hasher.GenerateRandomGuid()
            };

            _userTokenRepository.Add(userToken);
            _unitOfWork.Save();

            return userToken.SecretToken;
        }
    }
}