using SQLite;

namespace PasswordManager.Helpers;

public static class DbHelpers
{
    public static bool DoesDbExist() => File.Exists("../Passwords.db");

    public static SQLiteConnection GetDbConnection()
    {
        string dbPath = "../Passwords.db";
        SQLiteConnection db = new(dbPath);
        return db;
    }
}
