namespace PasswordManager.Services.Interfaces;

public interface IEncryptionService
{
    void DeriveKeysFromMasterPassword(string masterPassword, byte[] salt);

    (byte[] encryptedPassword, byte[] iv) EncryptPassword(string password, byte[] key);

    string DecryptPassword(byte[] iv, byte[] encryptedPassword, byte[] key);

    string RandomPasswordGenerator();

    byte[] GenerateSalt(int size);
}
