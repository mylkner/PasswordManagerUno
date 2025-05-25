namespace PasswordManager.Services;

public class EncryptionKeyService : IEncryptionKeyService
{
    public byte[]? EncryptionKey { get; set; }
}
