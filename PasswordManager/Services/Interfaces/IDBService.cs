namespace PasswordManager.Services.Interfaces;

public interface IDBService
{
    Task CreateDB(MasterPassword masterPassword, CancellationToken ct);
    Task<MasterPassword> GetPasswordHashAndSalt(CancellationToken ct);
    ValueTask<ImmutableList<PasswordPreview>> GetPasswords(string SearchTerm, CancellationToken ct);
    Task<PasswordEncrypted> GetEncryptedPassword(int id, CancellationToken ct);
    Task<PasswordPreview> AddPassword(
        string title,
        byte[] encryptedPassword,
        byte[] iv,
        CancellationToken ct
    );
    Task DeletePassword(int id, CancellationToken ct);
}
