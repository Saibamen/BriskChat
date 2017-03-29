using System;
using System.Security.Cryptography;
using TrollChat.BusinessLogic.Helpers.Interfaces;

namespace TrollChat.BusinessLogic.Helpers.Implementations
{
    public class Hasher : IHasher
    {
        private const int KeySize = 192; //256 chars
        private const int SaltSize = 96; //128 chars
        private const int Iterations = 10000;

        public string CreatePasswordHash(string plaintext, string salt)
        {
            if (string.IsNullOrEmpty(plaintext) || string.IsNullOrEmpty(salt)) return null;

            var byteArraySalt = Convert.FromBase64String(salt);
            var deriveBytes = new Rfc2898DeriveBytes(plaintext, byteArraySalt, Iterations);
            var key = deriveBytes.GetBytes(KeySize);

            return Convert.ToBase64String(key);
        }

        public string GenerateRandomGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public string GenerateRandomSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var buffor = new byte[SaltSize];
            rng.GetBytes(buffor);

            return Convert.ToBase64String(buffor);
        }
    }
}