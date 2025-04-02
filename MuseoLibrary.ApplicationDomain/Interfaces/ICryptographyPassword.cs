namespace MuseoLibrary.ApplicationDomain.Interfaces
{
    public interface ICryptographyPassword
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
