namespace PasswordManager.Services.Interfaces;

public interface IDBService
{
    ValueTask<bool> VerifyMasterPassword(string masterPassword, CancellationToken ct);
    ValueTask<IImmutableList<Password>> GetPasswords(CancellationToken ct);
}
