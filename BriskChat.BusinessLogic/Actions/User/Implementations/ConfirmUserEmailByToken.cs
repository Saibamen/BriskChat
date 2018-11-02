using System;
using System.Linq;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
{
    public class ConfirmUserEmailByToken : IConfirmUserEmailByToken
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmUserEmailByToken(IUserTokenRepository userTokenRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userTokenRepository = userTokenRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
            {
                return false;
            }

            var userToken = _userTokenRepository
                .FindBy(x => x.SecretToken == guid)
                .Include(x => x.User)
                .FirstOrDefault();

            if (userToken == null
                || userToken.User.EmailConfirmedOn != null
                || userToken.SecretTokenTimeStamp <= DateTime.UtcNow)
            {
                return false;
            }

            userToken.User.EmailConfirmedOn = DateTime.UtcNow;

            _userRepository.Edit(userToken.User);

            _userTokenRepository.Delete(userToken);
            _unitOfWork.Save();

            return true;
        }
    }
}