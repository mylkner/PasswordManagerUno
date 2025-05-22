namespace PasswordManager.Services.Interfaces;

public interface IDBService
{
    bool VerifyMasterPassword(IState<string> masterPassword);
    ValueTask<IImmutableList<Password>> GetPasswords(CancellationToken ct);
}
