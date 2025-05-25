using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Services;

public class EncryptionService : IEncryptionService
{
    public MasterPassword CreateMasterPasswordHashAndSalts(string masterPassword)
    {
        byte[] verSalt = EncryptionHelpers.GenerateSalt(32);
        byte[] encSalt = EncryptionHelpers.GenerateSalt(32);
        byte[] hash = EncryptionHelpers.GenerateHash(masterPassword, verSalt);

        return new MasterPassword()
        {
            MasterPasswordHash = hash,
            VerSalt = verSalt,
            EncSalt = encSalt,
        };
    }

    public void VerifyMasterPassword(string inputPassword, byte[] hash, byte[] salt)
    {
        byte[] inputPasswordHash = EncryptionHelpers.GenerateHash(inputPassword, salt);
        if (!inputPasswordHash.SequenceEqual(hash))
            throw new Exception(message: "Incorrect password");
    }

    public byte[] DeriveEncKeyFromMasterPassword(string masterPassword, byte[] encSalt) =>
        EncryptionHelpers.GenerateHash(masterPassword, encSalt);

    public (byte[] encryptedPassword, byte[] iv) EncryptPassword(string password, byte[] key)
    {
        byte[] iv = EncryptionHelpers.GenerateSalt(16);
        byte[] passwordInBytes = Encoding.UTF8.GetBytes(password);

        using Aes aes = Aes.Create();
        aes.IV = iv;
        aes.Key = key;

        using MemoryStream ms = new();
        using ICryptoTransform encryptor = aes.CreateEncryptor();
        using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);

        cs.Write(passwordInBytes, 0, passwordInBytes.Length);
        cs.FlushFinalBlock();

        return (ms.ToArray(), iv);
    }

    public string DecryptPassword(byte[] encryptedPassword, byte[] iv, byte[] key)
    {
        using Aes aes = Aes.Create();
        aes.IV = iv;
        aes.Key = key;

        using MemoryStream ms = new(encryptedPassword);
        using ICryptoTransform decryptor = aes.CreateDecryptor();
        using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
        using MemoryStream msPlain = new();
        cs.CopyTo(msPlain);

        return Encoding.UTF8.GetString(msPlain.ToArray());
    }

    public string RandomPasswordGenerator()
    {
        string validChars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+";
        Random rnd = new();
        int length = rnd.Next(16, 21);
        char[] password = new char[length];
        byte[] randomBytes = new byte[length];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        for (int i = 0; i < length; i++)
        {
            password[i] = validChars[randomBytes[i] % validChars.Length];
        }

        return new string(password);
    }
}
