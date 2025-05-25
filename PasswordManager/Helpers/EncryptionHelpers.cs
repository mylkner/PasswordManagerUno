using System.Security.Cryptography;

namespace PasswordManager.Helpers;

public static class EncryptionHelpers
{
    public static byte[] GenerateSalt(int size)
    {
        byte[] salt = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }

    public static byte[] GenerateHash(string toHash, byte[] salt)
    {
        using Rfc2898DeriveBytes hash = new(toHash, salt, 100000, HashAlgorithmName.SHA512);
        return hash.GetBytes(32);
    }
}
