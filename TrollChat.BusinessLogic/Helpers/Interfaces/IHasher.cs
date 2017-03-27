using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Helpers.Interfaces
{
    public interface IHasher : IAction
    {
        string CreateHash(string password, string salt);

        string GenerateRandomSalt();
    }
}