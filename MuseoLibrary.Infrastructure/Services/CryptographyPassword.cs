using MuseoLibrary.ApplicationDomain.Interfaces;

namespace MuseoLibrary.Infrastructure.Services
{
    public class CryptographyPassword : ICryptographyPassword
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
