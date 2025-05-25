namespace PasswordManager.Models.ViewModels;

public partial record MainModel(
    IDBService DBService,
    IEncryptionService EncryptionService,
    IEncryptionKeyService EncryptionKeyService,
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
            if (string.IsNullOrWhiteSpace(masterPassword))
                throw new Exception(message: "Password cannot be empty");

            MasterPassword hashAndSalts = DBService.GetPasswordHashAndSalt();
            EncryptionService.VerifyMasterPassword(
                masterPassword,
                hashAndSalts.MasterPasswordHash,
                hashAndSalts.VerSalt
            );
            byte[] encKey = EncryptionService.DeriveEncKeyFromMasterPassword(
                masterPassword,
                hashAndSalts.EncSalt
            );
            EncryptionKeyService.EncryptionKey = encKey;

            await SetResponse("Success - Redirecting...");
            await Task.Delay(TimeSpan.FromSeconds(2), ct);
            await _navigator.NavigateRouteAsync(this, "Passwords", cancellation: ct);
        }
        catch (Exception ex)
        {
            await SetResponse($"Error: {ex.Message}");
        }
        finally
        {
            await SetLoading(false);
        }
    }
}
