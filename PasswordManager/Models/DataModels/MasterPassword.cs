using SQLite;

namespace PasswordManager.Models.DataModels;

[Table("master_password")]
public class MasterPassword
{
    [Column("id")]
    [PrimaryKey]
    public int Id { get; set; }

    [Column("master_password_hash")]
    public byte[] MasterPasswordHash { get; set; } = [];

    [Column("ver_salt")]
    public byte[] VerSalt { get; set; } = [];

    [Column("enc_salt")]
    public byte[] EncSalt { get; set; } = [];
}
