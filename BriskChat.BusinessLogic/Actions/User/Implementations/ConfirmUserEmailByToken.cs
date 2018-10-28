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
        private readonly IUserRepository userRepository;
        private readonly IUserTokenRepository userTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmUserEmailByToken(IUserTokenRepository userTokenRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            this.userTokenRepository = userTokenRepository;
            this.userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            var userToken = userTokenRepository.FindBy(x => x.SecretToken == guid).Include(x => x.User).FirstOrDefault();

            if (userToken == null
                || userToken.User.EmailConfirmedOn != null
                || userToken.SecretTokenTimeStamp <= DateTime.UtcNow)
            {
                return false;
            }

            userToken.User.EmailConfirmedOn = DateTime.UtcNow;

            userRepository.Edit(userToken.User);

            userTokenRepository.Delete(userToken);
            _unitOfWork.Save();

            return true;
        }
    }
}