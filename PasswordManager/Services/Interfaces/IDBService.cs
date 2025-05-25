using SQLite;

namespace PasswordManager.Services.Interfaces;

public interface IDBService
{
    ValueTask CreateDB(MasterPassword masterPassword, CancellationToken ct);
    ValueTask<MasterPassword> GetPasswordHashAndSalt(CancellationToken ct);
    ValueTask<IImmutableList<Password>> GetPasswords(CancellationToken ct);
}
