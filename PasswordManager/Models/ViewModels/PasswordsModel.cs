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
}
