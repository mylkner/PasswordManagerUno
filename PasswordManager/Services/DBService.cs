using SQLite;

namespace PasswordManager.Services;

public class DBService : IDBService
{
    public void CreateDB(MasterPassword masterPassword)
    {
        using SQLiteConnection db = DbHelpers.GetDbConnection();
        db.CreateTable<MasterPassword>();
        db.CreateTable<Password>();
        if (db.Find<MasterPassword>(1) == null)
        {
            masterPassword.Id = 1;
            db.Insert(masterPassword);
        }
    }

    public MasterPassword GetPasswordHashAndSalt()
    {
        using SQLiteConnection db = DbHelpers.GetDbConnection();
        MasterPassword masterPasswordInfo = db.Find<MasterPassword>(1);
        return masterPasswordInfo;
    }

    public IImmutableList<Password> GetPasswords()
    {
        using SQLiteConnection db = DbHelpers.GetDbConnection();
        ImmutableList<Password> passwords = db.Table<Password>().ToImmutableList();
        return passwords;
    }
}
