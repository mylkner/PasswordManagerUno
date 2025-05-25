using SQLite;

namespace PasswordManager.Services;

public class DBService : IDBService
{
    public static bool DoesDbExist() => File.Exists("../Passwords.db");

    public void CreateDB(MasterPassword masterPassword)
    {
        using SQLiteConnection db = GetDbConnection();
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
        using SQLiteConnection db = GetDbConnection();
        MasterPassword masterPasswordInfo = db.Find<MasterPassword>(1);
        return masterPasswordInfo;
    }

    public IImmutableList<Password> GetPasswords()
    {
        using SQLiteConnection db = GetDbConnection();
        ImmutableList<Password> passwords = db.Table<Password>().ToImmutableList();
        return passwords;
    }

    private static SQLiteConnection GetDbConnection()
    {
        string dbPath = "../Passwords.db";
        SQLiteConnection db = new(dbPath);
        return db;
    }
}
