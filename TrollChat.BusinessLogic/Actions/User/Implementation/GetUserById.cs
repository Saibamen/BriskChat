using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementation
{
    public class GetUserById : IGetUserById
    {
        private readonly IUserRepository userRepository;

        public GetUserById(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public DataAccess.Models.User Invoke(int id)
        {
            var dbUser = userRepository.GetById(id);
            if (dbUser == null)
            {
                return null;
            }

            var user = AutoMapper.Mapper.Map<DataAccess.Models.User>(dbUser);
            return user;
        }
    }
}