namespace PasswordManager.Models.ViewModels;

public partial record MainModel(IDBService DBService, INavigator Navigator)
{
    private readonly INavigator _navigator = Navigator;
    public IState<string> MasterPassword => State<string>.Value(this, () => "");
    public IState<string> VerificationResponse => State<string>.Empty(this);
    public IState<bool> Loading => State<bool>.Value(this, () => false);

    public async ValueTask VerifyButtonCommand(CancellationToken ct)
    {
        async ValueTask SetLoading(bool isLoading) =>
            await Loading.UpdateDataAsync(_ => isLoading, ct);
        async ValueTask SetResponse(string? message) =>
            await VerificationResponse.UpdateAsync(_ => message, ct);

        await SetLoading(true);
        await SetResponse(null);

        try
        {
            string? masterPassword = await MasterPassword;
            if (masterPassword is null)
            {
                await SetResponse("Password cannot be null");
                return;
            }

            if (!await DBService.VerifyMasterPassword(masterPassword, ct))
            {
                await SetResponse("Error");
                return;
            }

            await SetResponse("Success");
            await _navigator.NavigateRouteAsync(this, "Passwords", cancellation: ct);
        }
        finally
        {
            await SetLoading(false);
        }
    }
}
