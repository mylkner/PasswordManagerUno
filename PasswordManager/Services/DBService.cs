namespace PasswordManager.Services;

public class DBService : IDBService
{
    public static bool DoesDbExist()
    {
        return true;
    }

    public async ValueTask CreateDB(
        byte[] masterPasswordHash,
        byte[] verSalt,
        byte[] encSalt,
        CancellationToken ct
    )
    {
        await Task.Delay(TimeSpan.FromSeconds(2), ct);
    }

    public async ValueTask<Dictionary<string, byte[]>> GetPasswordHashAndSalt(CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), ct);
        return new Dictionary<string, byte[]>
        {
            { "hash", [] },
            { "verSalt", [] },
            { "encSalt", [] },
        };
    }

    public async ValueTask<IImmutableList<Password>> GetPasswords(CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), ct);
        Password[] passwords = [new Password("a"), new Password("b")];
        return passwords.ToImmutableList();
    }
}
