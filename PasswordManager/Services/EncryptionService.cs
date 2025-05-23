using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Services;

public class EncryptionService : IEncryptionService
{
    public void DeriveKeysFromMasterPassword(string masterPassword, byte[] salt)
    {
        using Rfc2898DeriveBytes pbkdf2 = new(
            masterPassword,
            salt,
            100000,
            HashAlgorithmName.SHA512
        );
    }

    public (byte[] encryptedPassword, byte[] iv) EncryptPassword(string password, byte[] key)
    {
        byte[] iv = GenerateSalt(16);
        byte[] passwordInBytes = Encoding.UTF8.GetBytes(password);

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using MemoryStream ms = new();
        using ICryptoTransform encryptor = aes.CreateEncryptor();
        using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);

        cs.Write(passwordInBytes, 0, passwordInBytes.Length);
        cs.FlushFinalBlock();

        return (ms.ToArray(), iv);
    }

    public string DecryptPassword(byte[] iv, byte[] encryptedPassword, byte[] key)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using MemoryStream ms = new(encryptedPassword);
        using ICryptoTransform decryptor = aes.CreateDecryptor();
        using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
        using StreamReader sr = new(cs);

        return sr.ReadToEnd();
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

    public byte[] GenerateSalt(int size)
    {
        byte[] salt = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }
}
