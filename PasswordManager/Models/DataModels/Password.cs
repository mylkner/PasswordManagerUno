using SQLite;

namespace PasswordManager.Models.DataModels;

[Table("passwords")]
public class Password
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; } = "";

    [Column("iv")]
    public byte[] Iv { get; set; } = [];

    [Column("encrypted_password")]
    public byte[] EncryptedPassword { get; set; } = [];
}

public class PasswordPreview(int id, string title)
{
    public int Id { get; set; } = id;
    public string Title { get; set; } = title;
}

public class PasswordEncrypted(byte[] iv, byte[] encryptedPassword)
{
    public byte[] Iv { get; set; } = iv;
    public byte[] EncryptedPassword { get; set; } = encryptedPassword;
}
