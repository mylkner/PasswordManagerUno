namespace PasswordManager.Models.ViewModels;

public partial record PasswordsModel(
    IDBService DBService,
    IEncryptionService EncryptionService,
    IEncryptionKeyService EncryptionKeyService
)
{
    private readonly byte[] _encKey = EncryptionKeyService.EncryptionKey;
    public IListState<Password> Passwords =>
        ListState<Password>.Async(this, DBService.GetPasswords);

    public void DecryptPassword(Password password)
    {
        try
        {
            string decryptedPassword = EncryptionService.DecryptPassword(
                password.EncryptedPassword,
                password.Iv,
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
            (byte[] encryptedPassword, byte[] iv) = EncryptionService.EncryptPassword(
                "awd",
                _encKey
            );
            Password newPassword = await DBService.AddPassword("Tile", encryptedPassword, iv);
            await Passwords.AddAsync(newPassword, ct);
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
            await DBService.DeletePassword(id);
            await Passwords.RemoveAllAsync(match: pwd => pwd.Id == id, ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
