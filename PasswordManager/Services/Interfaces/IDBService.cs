using SQLite;

namespace PasswordManager.Services.Interfaces;

public interface IDBService
{
    void CreateDB(MasterPassword masterPassword);
    MasterPassword GetPasswordHashAndSalt();
    IImmutableList<Password> GetPasswords();
}
