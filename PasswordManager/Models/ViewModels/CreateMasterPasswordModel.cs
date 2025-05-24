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

            if (mP is null || mPR is null)
            {
                await SetResponse("Neither value can be null");
                return;
            }

            if (mP != mPR)
            {
                await SetResponse("Passwords do not match");
                return;
            }

            await DBService.CreateDB(ct);
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
