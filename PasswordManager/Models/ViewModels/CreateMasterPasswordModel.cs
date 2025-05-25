namespace PasswordManager.Models.ViewModels;

public partial record CreateMasterPasswordModel(
    IDBService DBService,
    IEncryptionService EncryptionService,
    IEncryptionKeyService EncryptionKeyService,
    INavigator Navigator
)
{
    private readonly INavigator _navigator = Navigator;
    public IState<string> MasterPassword => State<string>.Value(this, () => "");
    public IState<string> MasterPasswordReEntered => State<string>.Value(this, () => "");
    public IState<string> CreationResponse => State<string>.Empty(this);
    public IState<bool> Loading => State<bool>.Value(this, () => false);

    public async ValueTask SetMasterPassword(CancellationToken ct)
    {
        async ValueTask SetLoading(bool isLoading) =>
            await Loading.UpdateDataAsync(_ => isLoading, ct);
        async ValueTask SetResponse(string? message) =>
            await CreationResponse.UpdateAsync(_ => message, ct);

        await SetLoading(true);
        await SetResponse(null);

        try
        {
            string? mP = await MasterPassword;
            string? mPR = await MasterPasswordReEntered;

            if (string.IsNullOrWhiteSpace(mP) || string.IsNullOrWhiteSpace(mPR))
                throw new Exception(message: "Values cannot be empty");

            if (mP != mPR)
                throw new Exception(message: "Passwords do not match");

            Dictionary<string, byte[]> hashAndSalts =
                EncryptionService.CreateMasterPasswordHashAndSalts(mP);
            await DBService.CreateDB(
                hashAndSalts["hash"],
                hashAndSalts["verSalt"],
                hashAndSalts["encSalt"],
                ct
            );
            byte[] encKey = EncryptionService.DeriveEncKeyFromMasterPassword(
                mP,
                hashAndSalts["encSalt"]
            );
            EncryptionKeyService.EncryptionKey = encKey;

            await SetResponse("Created - Redirecting...");
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
