namespace PasswordManager.Models.ViewModels;

public partial record MainModel(
    IDBService DBService,
    IEncryptionService EncryptionService,
    INavigator Navigator
)
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

            Dictionary<string, byte[]> hashAndSalts = await DBService.GetPasswordHashAndSalt(ct);

            if (
                !EncryptionService.VerifyMasterPassword(
                    masterPassword,
                    hashAndSalts["hash"],
                    hashAndSalts["verSalt"]
                )
            )
            {
                await SetResponse("Incorrect password");
                return;
            }

            byte[] encKey = EncryptionService.DeriveEncKeyFromMasterPassword(
                masterPassword,
                hashAndSalts["encSalt"]
            );
            await SetResponse("Success - Redirecting...");
            await Task.Delay(TimeSpan.FromSeconds(2), ct);
            await _navigator.NavigateRouteAsync(this, "Passwords", cancellation: ct);
        }
        finally
        {
            await SetLoading(false);
        }
    }
}
