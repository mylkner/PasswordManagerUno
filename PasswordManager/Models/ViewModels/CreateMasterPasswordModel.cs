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

            MasterPassword hashAndSalts = EncryptionService.CreateMasterPasswordHashAndSalts(mP);
            await DBService.CreateDB(hashAndSalts, ct);
            byte[] encKey = EncryptionService.DeriveEncKeyFromMasterPassword(
                mP,
                hashAndSalts.EncSalt
            );
            EncryptionKeyService.EncryptionKey = encKey;

            await SetResponse("Created - Redirecting...");
            await MasterPassword.UpdateAsync(_ => "", ct);
            await MasterPasswordReEntered.UpdateAsync(_ => "", ct);
            await Task.Delay(TimeSpan.FromSeconds(1), ct);

            await _navigator.NavigateViewModelAsync<PasswordsViewModel>(
                this,
                qualifier: Qualifiers.ClearBackStack,
                cancellation: ct
            );
        }
        catch (Exception ex)
        {
            await SetResponse($"Error: {ex.Message}");
            await MasterPassword.UpdateAsync(_ => "", ct);
            await MasterPasswordReEntered.UpdateAsync(_ => "", ct);
            await SetLoading(false);
        }
    }
}
