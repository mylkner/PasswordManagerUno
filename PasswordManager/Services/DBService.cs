namespace PasswordManager.Services;

public class DBService : IDBService
{
    public static bool DoesDbExist()
    {
        return true;
    }

    public async ValueTask<bool> CreateDB(CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), ct);
        return true;
    }

    public async ValueTask<bool> VerifyMasterPassword(string masterPassword, CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), ct);
        return true;
    }

    public async ValueTask<IImmutableList<Password>> GetPasswords(CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), ct);
        Password[] passwords = [new Password("a"), new Password("b")];
        return passwords.ToImmutableList();
    }
}
