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

    private void IncrementAttempt()
    {
        if (_attempts < 3)
            _attempts++;
        else
        {
            _attempts = 0;
            _timer = 30;
            throw new Exception(
                message: $"Too many attempts. Please wait {_timer} seconds before trying again"
            );
        }
    }

    private async Task Timer(CancellationToken ct)
    {
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

            IncrementAttempt();

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

            //#if __ANDROID__ || __IOS__
            //private void GotoNextPage(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(PasswordsPage));
            //#else
            await _navigator.NavigateViewModelAsync<PasswordsViewModel>(
                this,
                qualifier: Qualifiers.ClearBackStack,
                cancellation: ct
            );
            //#endif
        }
        catch (Exception ex)
        {
            await SetResponse($"Error: {ex.Message}");
            await MasterPassword.UpdateAsync(_ => "", ct);
            await SetLoading(false);
            if (ex.Message.Contains("Too many attempts"))
                await Timer(ct);
        }
    }
}
