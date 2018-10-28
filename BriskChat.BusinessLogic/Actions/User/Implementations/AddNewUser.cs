using System.Linq;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Helpers.Implementations;
using BriskChat.BusinessLogic.Helpers.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
{
    public class AddNewUser : IAddNewUser
    {
        private readonly IUserRepository userRepository;
        private readonly IUserTokenRepository userTokenRepository;
        private readonly IHasher hasher;
        private readonly IUnitOfWork _unitOfWork;

        public AddNewUser(IUserRepository userRepository,
            IUserTokenRepository userTokenRepository, IUnitOfWork unitOfWork, IHasher hasher = null)
        {
            this.userRepository = userRepository;
            this.userTokenRepository = userTokenRepository;
            _unitOfWork = unitOfWork;
            this.hasher = hasher ?? new Hasher();
        }

        // TODO: Check for user in domain || Not necessary if we checking email??
        public DataAccess.Models.User Invoke(Models.UserModel user)
        {
            if (!user.IsValid() || userRepository.FindBy(x => x.Email == user.Email).Count() > 0)
            {
                return null;
            }

            var newUser = AutoMapper.Mapper.Map<DataAccess.Models.User>(user);
            newUser.PasswordSalt = hasher.GenerateRandomSalt();
            newUser.PasswordHash = hasher.CreatePasswordHash(user.Password, newUser.PasswordSalt);

            userRepository.Add(newUser);

            var newUserToken = new DataAccess.Models.UserToken
            {
                User = newUser,
                SecretToken = hasher.GenerateRandomGuid()
            };

            userTokenRepository.Add(newUserToken);
            _unitOfWork.Save();

            return newUser;
        }
    }
}