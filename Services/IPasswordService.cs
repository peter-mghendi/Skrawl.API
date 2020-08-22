namespace Skrawl.API.Services
{
    public interface IPasswordService
    {
        public bool VerifyHash(byte[] password, byte[] salt, byte[] hash);
        public byte[] HashPassword(byte[] password, ref byte[] salt);
    }
}