namespace PasswordManager.Services.Interfaces;

public interface IEncryptionService
{
    MasterPassword CreateMasterPasswordHashAndSalts(string masterPassword);
    void VerifyMasterPassword(string masterPassword, byte[] hash, byte[] salt);
    byte[] DeriveEncKeyFromMasterPassword(string inputPassword, byte[] salt);
    (byte[] encryptedPassword, byte[] iv) EncryptPassword(string password, byte[] key);
    string DecryptPassword(byte[] encryptedPassword, byte[] iv, byte[] key);
    string RandomPasswordGenerator();
}
