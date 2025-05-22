namespace PasswordManager.Models;

public partial record MainModel(IDBService DBService, INavigator Navigator)
{
    private readonly INavigator _navigator = Navigator;
    public IState<string> MasterPassword => State<string>.Value(this, () => "");

    public void VerifyButtonCommand()
    {
        if (DBService.VerifyMasterPassword(MasterPassword))
        {
            Console.WriteLine("A");
            _navigator.NavigateRouteAsync(this, "Passwords");
        }
    }
}
