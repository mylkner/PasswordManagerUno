using SQLite;

namespace PasswordManager.Models.DataModels;

public class MasterPassword
{
    public required byte[] MasterPasswordHash { get; set; }
    public required byte[] VerSalt { get; set; }
    public required byte[] EncSalt { get; set; }
}
