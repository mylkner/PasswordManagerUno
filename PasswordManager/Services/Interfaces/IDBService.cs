namespace PasswordManager.Services.Interfaces;

public interface IDBService
{
    Task CreateDB(MasterPassword masterPassword, CancellationToken ct);
    Task<MasterPassword> GetPasswordHashAndSalt(CancellationToken ct);
    ValueTask<IImmutableList<Password>> GetPasswords(CancellationToken ct);
    Task<Password> AddPassword(string title, byte[] encryptedPassword, byte[] iv);
    Task DeletePassword(int id);
}
