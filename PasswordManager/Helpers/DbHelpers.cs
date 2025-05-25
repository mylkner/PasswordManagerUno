using SQLite;

namespace PasswordManager.Helpers;

public static class DbHelpers
{
    private static readonly StorageFolder _appDataFolder = ApplicationData.Current.LocalFolder;
    private static readonly string _dataFolderPath = System.IO.Path.Combine(
        _appDataFolder.Path,
        "Passwords.db"
    );

    public static bool DoesDbExist() => File.Exists(_dataFolderPath);

    public static void CreateDirectory() => Directory.CreateDirectory(_appDataFolder.Path);

    public static SQLiteAsyncConnection GetDbConnection()
    {
        string dbPath = _dataFolderPath;
        return new(dbPath);
    }
}
