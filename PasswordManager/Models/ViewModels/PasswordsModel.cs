using System.Threading.Tasks;

namespace PasswordManager.Models.ViewModels;

public partial record PasswordsModel(
    IDBService DBService,
    IEncryptionService EncryptionService,
    IEncryptionKeyService EncryptionKeyService
)
{
    private readonly byte[] _encKey = EncryptionKeyService.EncryptionKey;
    public IState<string> PasswordToAdd => State<string>.Empty(this);
    public IState<string> SearchTerm => State<string>.Value(this, () => "");
    public IListFeed<PasswordPreview> Passwords =>
        SearchTerm.SelectAsync(DBService.GetPasswords).AsListFeed();

    public async Task DecryptPassword(int id, CancellationToken ct)
    {
        try
        {
            PasswordEncrypted pwdEnc = await DBService.GetEncryptedPassword(id, ct);
            string decryptedPassword = EncryptionService.DecryptPassword(
                pwdEnc.EncryptedPassword,
                pwdEnc.Iv,
                _encKey
            );
            Console.WriteLine(decryptedPassword);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task AddPassword(CancellationToken ct)
    {
        try
        {
            string? passwordToAdd = await PasswordToAdd;
            (byte[] encryptedPassword, byte[] iv) = EncryptionService.EncryptPassword(
                passwordToAdd!,
                _encKey
            );
            Password newPassword = await DBService.AddPassword("Tile", encryptedPassword, iv, ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public async Task DeletePassword(int id, CancellationToken ct)
    {
        try
        {
            await DBService.DeletePassword(id, ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
