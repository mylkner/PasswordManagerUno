using SQLite;

namespace PasswordManager.Models.DataModels;

public class Password
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public required string Title { get; set; }
    public required byte[] Iv { get; set; }
    public required byte[] EncryptedPassword { get; set; }
}
