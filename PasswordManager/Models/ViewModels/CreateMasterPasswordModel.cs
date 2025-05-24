namespace PasswordManager.Models.ViewModels;

public partial record CreateMasterPasswordModel(
    IDBService DBService,
    IEncryptionService EncryptionService,
    INavigator Navigator
)
{
    private readonly INavigator _navigator = Navigator;
    public IState<string> MasterPassword => State<string>.Value(this, () => "");
    public IState<string> MasterPasswordReEntered => State<string>.Value(this, () => "");
    public IState<string> Response => State<string>.Empty(this);
    public IState<bool> Loading => State<bool>.Value(this, () => false);

    public async ValueTask SetMasterPassword() { }
}
