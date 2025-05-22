using System.Threading.Tasks;

namespace PasswordManager.Services;

class DBService : IDBService
{
    public bool VerifyMasterPassword(IState<string> masterPassword)
    {
        return true;
    }

    public async ValueTask<IImmutableList<Password>> GetPasswords(CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), ct);
        Password[] passwords = [new Password("a"), new Password("b")];
        return passwords.ToImmutableList();
    }
}
