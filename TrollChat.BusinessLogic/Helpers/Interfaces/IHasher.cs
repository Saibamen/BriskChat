using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Helpers.Interfaces
{
    public interface IHasher : IAction
    {
        string CreatePasswordHash(string password, string salt);

        string GenerateRandomSalt();

        string GenerateRandomGuid();
    }
}