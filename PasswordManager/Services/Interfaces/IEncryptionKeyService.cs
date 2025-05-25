namespace PasswordManager.Services.Interfaces;

public interface IEncryptionKeyService
{
    byte[]? EncryptionKey { get; set; }
}
