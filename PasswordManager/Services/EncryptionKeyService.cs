namespace PasswordManager.Services;

public class EncryptionKeyService : IEncryptionKeyService
{
    private byte[]? _encryptionKey;

    public byte[]? EncryptionKey
    {
        get => _encryptionKey;
        set
        {
            if (_encryptionKey == null)
                _encryptionKey = value;
            else
                throw new Exception(message: "Encryption key already set");
        }
    }
}
