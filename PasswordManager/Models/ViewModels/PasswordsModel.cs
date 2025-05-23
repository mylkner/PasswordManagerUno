namespace PasswordManager.Models.ViewModels;

public partial record PasswordsModel(IDBService DBService, IEncryptionService EncryptionService)
{
    public IListFeed<Password> Passwords => ListFeed.Async(DBService.GetPasswords);
}
