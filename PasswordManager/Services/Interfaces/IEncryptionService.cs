namespace PasswordManager.Services.Interfaces;

public interface IEncryptionService
{
    Dictionary<string, byte[]> CreateMasterPasswordHashAndSalts(string masterPassword);
    bool VerifyMasterPassword(string masterPassword, byte[] hash, byte[] salt);
    byte[] DeriveEncKeyFromMasterPassword(string masterPassword, byte[] salt);
    (byte[] encryptedPassword, byte[] iv) EncryptPassword(string password, byte[] key);
    string DecryptPassword(byte[] iv, byte[] encryptedPassword, byte[] key);
    string RandomPasswordGenerator();
}
