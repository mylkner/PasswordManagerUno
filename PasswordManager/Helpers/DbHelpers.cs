using SQLite;
using Windows.Storage.Pickers;

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

    public static async Task ImportDb()
    {
        await GetDbConnection().CloseAsync();
        StorageFile dbFile = await _appDataFolder.GetFileAsync("Passwords.db");
        FileOpenPicker fop = new() { SuggestedStartLocation = PickerLocationId.Downloads };
        fop.FileTypeFilter.Add(".db");

        StorageFile pickedFile = await fop.PickSingleFileAsync();
        if (pickedFile != null)
        {
            await pickedFile.CopyAndReplaceAsync(dbFile);
        }
        else
        {
            throw new Exception("Operation cancelled");
        }
    }

    public static async Task ExportDb()
    {
        await GetDbConnection().CloseAsync();
        StorageFile dbFile = await _appDataFolder.GetFileAsync("Passwords.db");
        FileSavePicker fsp = new()
        {
            SuggestedStartLocation = PickerLocationId.Downloads,
            SuggestedFileName = "Passwords_Backup",
        };
        fsp.FileTypeChoices.Add("Database", [".db"]);

        StorageFile file = await fsp.PickSaveFileAsync();
        if (file != null)
        {
            await dbFile.CopyAndReplaceAsync(file);
        }
        else
        {
            throw new Exception("Operation cancelled");
        }
    }
}
