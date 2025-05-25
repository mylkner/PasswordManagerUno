using SQLite;

namespace PasswordManager.Models.DataModels;

[Table("Password")]
public class Password
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; }

    [Column("iv")]
    public byte[] Iv { get; set; }

    [Column("encrypted_password")]
    public byte[] EncryptedPassword { get; set; }
}
