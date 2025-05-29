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

    public async ValueTask<ImmutableList<PasswordPreview>> GetPasswords(
        string SearchTerm,
        CancellationToken ct
    )
    {
        SQLiteAsyncConnection db = DbHelpers.GetDbConnection();
        List<Password> passwords = string.IsNullOrWhiteSpace(SearchTerm)
            ? await db.QueryAsync<Password>("SELECT id, title FROM passwords")
            : await db.QueryAsync<Password>(
                "SELECT id, title FROM passwords WHERE title LIKE LOWER(?)",
                $"%{SearchTerm.ToLower()}%"
            );
        return passwords
            .Select(pwd => new PasswordPreview(pwd.Id, pwd.Title))
            .OrderBy(pwd => pwd.Title)
            .ToImmutableList();
    }

    public async Task<PasswordEncrypted> GetEncryptedPassword(int id, CancellationToken ct)
    {
        SQLiteAsyncConnection db = DbHelpers.GetDbConnection();
        List<Password> pwdEnc = await db.QueryAsync<Password>(
            "SELECT iv, encrypted_password FROM passwords WHERE id = ?",
            id
        );
        return pwdEnc.Select(pwd => new PasswordEncrypted(pwd.Iv, pwd.EncryptedPassword)).First();
    }

    public async Task<PasswordPreview> AddPassword(
        string title,
        byte[] encryptedPassword,
        byte[] iv,
        CancellationToken ct
    )
    {
        SQLiteAsyncConnection db = DbHelpers.GetDbConnection();
        int checkForDuplicates = await db.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM passwords WHERE title = ?",
            title
        );
        if (checkForDuplicates > 0)
            throw new Exception(message: $"Password with title '{title}' already exists");

        Password newPassword = new()
        {
            Title = title,
            Iv = iv,
            EncryptedPassword = encryptedPassword,
        };
        await db.InsertAsync(newPassword);
        return new PasswordPreview(newPassword.Id, newPassword.Title);
    }

    public async Task DeletePassword(int id, CancellationToken ct)
    {
        SQLiteAsyncConnection db = DbHelpers.GetDbConnection();
        await db.DeleteAsync<Password>(id);
    }
}
