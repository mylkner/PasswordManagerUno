namespace PasswordManager.Services.Interfaces;

public interface IDBService
{
    ValueTask<bool> CreateDB(CancellationToken ct);
    ValueTask<bool> VerifyMasterPassword(string masterPassword, CancellationToken ct);
    ValueTask<IImmutableList<Password>> GetPasswords(CancellationToken ct);
}
