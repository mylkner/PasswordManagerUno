namespace PasswordManager.Models.ViewModels;

public partial record MainModel(
    IDBService DBService,
    IEncryptionService EncryptionService,
    IEncryptionKeyService EncryptionKeyService,
    INavigator Navigator
)
{
    private readonly INavigator _navigator = Navigator;
    private int _attempts = 0;
    private int _timer = 0;
    public IState<string> MasterPassword => State<string>.Value(this, () => "");
    public IState<string> VerificationResponse => State<string>.Empty(this);
    public IState<bool> Loading => State<bool>.Value(this, () => false);

    private async Task Timer(CancellationToken ct)
    {
        _timer = 30;
        await VerificationResponse.UpdateAsync(
            _ => $"Too many attempts. Please wait {_timer} seconds before trying again",
            ct
        );
        while (_timer > 0)
        {
            await Task.Delay(1000, ct);
            _timer -= 1;
            await VerificationResponse.UpdateAsync(
                _ => $"Too many attempts. Please wait {_timer} seconds before trying again",
                ct
            );
        }
        await VerificationResponse.UpdateAsync(_ => $"", ct);
    }

    public async ValueTask VerifyButtonCommand(CancellationToken ct)
    {
        async ValueTask SetLoading(bool isLoading) => await Loading.UpdateAsync(_ => isLoading, ct);
        async ValueTask SetResponse(string? message) =>
            await VerificationResponse.UpdateAsync(_ => message, ct);

        await SetLoading(true);
        await SetResponse(null);

        try
        {
            string? masterPassword = await MasterPassword;
            if (string.IsNullOrWhiteSpace(masterPassword))
                throw new Exception(message: "Password cannot be empty");

            MasterPassword hashAndSalts = await DBService.GetPasswordHashAndSalt(ct);
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
            await MasterPassword.UpdateAsync(_ => "", ct);
            await Task.Delay(TimeSpan.FromSeconds(1), ct);

            await _navigator.NavigateViewModelAsync<PasswordsViewModel>(
                this,
                qualifier: Qualifiers.ClearBackStack,
                cancellation: ct
            );
        }
        catch (Exception ex)
        {
            await SetLoading(false);
            await MasterPassword.UpdateAsync(_ => "", ct);
            _attempts++;
            if (_attempts == 3)
            {
                await Timer(ct);
                _attempts = 0;
            }
            else
            {
                await SetResponse($"Error: {ex.Message}");
            }
        }
    }
}
