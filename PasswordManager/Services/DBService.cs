using SQLite;

namespace PasswordManager.Services;

public class DBService : IDBService
{
    public async Task CreateDB(MasterPassword masterPassword, CancellationToken ct)
    {
        SQLiteAsyncConnection db = DbHelpers.GetDbConnection();
        await db.CreateTableAsync<MasterPassword>();
        await db.CreateTableAsync<Password>();
        if (await db.FindAsync<MasterPassword>(1) == null)
        {
            masterPassword.Id = 1;
            await db.InsertAsync(masterPassword);
        }
    }

    public async Task<MasterPassword> GetPasswordHashAndSalt(CancellationToken ct)
    {
        SQLiteAsyncConnection db = DbHelpers.GetDbConnection();
        MasterPassword masterPasswordInfo = await db.FindAsync<MasterPassword>(1);
        return masterPasswordInfo;
    }

    public async ValueTask<IImmutableList<Password>> GetPasswords(CancellationToken ct)
    {
        SQLiteAsyncConnection db = DbHelpers.GetDbConnection();
        List<Password> passwords = await db.Table<Password>().ToListAsync();
        return passwords.ToImmutableList();
    }

    public async Task<Password> AddPassword(string title, byte[] iv, byte[] encryptedPassword)
    {
        SQLiteAsyncConnection db = DbHelpers.GetDbConnection();
        Password newPassword = new()
        {
            Title = title,
            Iv = iv,
            EncryptedPassword = encryptedPassword,
        };
        await db.InsertAsync(newPassword);
        return newPassword;
    }

    public async Task DeletePassword(int id)
    {
        SQLiteAsyncConnection db = DbHelpers.GetDbConnection();
        await db.DeleteAsync<Password>(id);
    }
}
