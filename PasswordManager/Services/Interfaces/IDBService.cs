using SQLite;

namespace PasswordManager.Services.Interfaces;

public interface IDBService
{
    Task CreateDB(MasterPassword masterPassword, CancellationToken ct);
    Task<MasterPassword> GetPasswordHashAndSalt(CancellationToken ct);
    ValueTask<IImmutableList<Password>> GetPasswords(CancellationToken ct);
    Task AddPassword(string title, byte[] iv, byte[] encryptedPassword);
    Task DeletePassword(int id);
}
