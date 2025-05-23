namespace PasswordManager.Models;

public partial record MainModel(IDBService DBService, INavigator Navigator)
{
    private readonly INavigator _navigator = Navigator;
    public IState<string> MasterPassword => State<string>.Value(this, () => "");
    public IState<string> VerificationResponse => State<string>.Empty(this);
    public IState<Color> VerificationResponseColor => State<Color>.Empty(this);

    public async ValueTask VerifyButtonCommand(CancellationToken ct)
    {
        await UpdateVerRes(null, null, ct);
        string? masterPassword = await MasterPassword;

        if (masterPassword is null)
        {
            await UpdateVerRes("Password cannot be null", Colors.Red, ct);
            return;
        }

        if (await DBService.VerifyMasterPassword(masterPassword, ct))
        {
            await UpdateVerRes("Success", Colors.Green, ct);
            await _navigator.NavigateRouteAsync(this, "Passwords", cancellation: ct);
        }
        else
        {
            await UpdateVerRes("Error", Colors.Green, ct);
        }
    }

    private async ValueTask UpdateVerRes(string? message, Color? color, CancellationToken ct)
    {
        await VerificationResponse.UpdateAsync(current => message, ct);
        await VerificationResponseColor.UpdateAsync(current => color, ct);
    }
}
