using System.Linq;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace Skrawl.API.Services
{
    public class PasswordService : IPasswordService
    {
        public bool VerifyHash(byte[] password, byte[] salt, byte[] hash) => 
            hash.SequenceEqual(HashPassword(password, ref salt));

        public byte[] HashPassword(byte[] password, ref byte[] salt)
        {
            salt ??= CreateSalt();

            var argon2 = new Argon2id(password);
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8;
            argon2.Iterations = 4;
            argon2.MemorySize = 1024 * 1024;

            return argon2.GetBytes(16);
        }

        private byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }
    }
}