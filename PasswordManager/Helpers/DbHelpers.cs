using SQLite;

namespace PasswordManager.Helpers;

public static class DbHelpers
{
    private static string filepath = "../Passwords.db";

    public static bool DoesDbExist() => File.Exists(filepath);

    public static SQLiteConnection GetDbConnection()
    {
        string dbPath = filepath;
        SQLiteConnection db = new(dbPath);
        return db;
    }
}
