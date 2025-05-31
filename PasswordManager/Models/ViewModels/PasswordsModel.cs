using Windows.ApplicationModel.DataTransfer;

namespace PasswordManager.Models.ViewModels;

public partial record PasswordsModel(
    IDBService DBService,
    IEncryptionService EncryptionService,
    IEncryptionKeyService EncryptionKeyService
)
{
    private readonly byte[] _encKey = EncryptionKeyService.EncryptionKey;
    public IState<string> TitleOfPasswordToAdd => State<string>.Empty(this);
    public IState<string> PasswordToAdd => State<string>.Empty(this);
    public IState<string> SearchTerm => State<string>.Value(this, () => "");
    public IState<string> Response => State<string>.Empty(this);

    public IListFeed<PasswordPreview> PasswordsFeed =>
        SearchTerm.SelectAsync(DBService.GetPasswords).AsListFeed();
    public IListState<PasswordPreview> Passwords => ListState.FromFeed(this, PasswordsFeed);

    public async Task GeneratePassword(CancellationToken ct) =>
        await PasswordToAdd.UpdateAsync(_ => EncryptionService.RandomPasswordGenerator(), ct);

    public async Task DecryptPassword(PasswordPreview pwd, CancellationToken ct)
    {
        try
        {
            await Response.UpdateAsync(_ => "", ct);
            PasswordEncrypted pwdEnc = await DBService.GetEncryptedPassword(pwd.Id, ct);
            string decryptedPassword = EncryptionService.DecryptPassword(
                pwdEnc.EncryptedPassword,
                pwdEnc.Iv,
                _encKey
            );

            DataPackage data = new();
            data.SetText(decryptedPassword);
            Clipboard.SetContent(data);
            await Response.UpdateAsync(_ => $"Password copied '{pwd.Title}' to clipboard", ct);
        }
        catch (Exception ex)
        {
            await Response.UpdateAsync(_ => $"Error: {ex.Message}", ct);
        }
    }

    public async Task AddPassword(CancellationToken ct)
    {
        try
        {
            await SearchTerm.UpdateAsync(_ => "", ct);
            await Response.UpdateAsync(_ => "", ct);
            string? titleOfPasswordToAdd = await TitleOfPasswordToAdd;
            string? passwordToAdd = await PasswordToAdd;

            if (
                string.IsNullOrWhiteSpace(titleOfPasswordToAdd)
                || string.IsNullOrWhiteSpace(passwordToAdd)
            )
                throw new Exception(message: "Values cannot be empty");

            if (titleOfPasswordToAdd.Length > 20)
                throw new Exception(message: "Title may not be greater than 20 characters");

            if (passwordToAdd.Length > 21)
                throw new Exception(message: "Password may not be greater than 21 characters");

            (byte[] encryptedPassword, byte[] iv) = EncryptionService.EncryptPassword(
                passwordToAdd!,
                _encKey
            );
            PasswordPreview newPwd = await DBService.AddPassword(
                titleOfPasswordToAdd!,
                encryptedPassword,
                iv,
                ct
            );
            await Passwords.UpdateAsync(
                updater: pwds => pwds.Add(newPwd).OrderBy(pwd => pwd.Title).ToImmutableList(),
                ct
            );
            await Response.UpdateAsync(_ => $"Password '{titleOfPasswordToAdd}' added", ct);
        }
        catch (Exception ex)
        {
            await Response.UpdateAsync(_ => $"Error: {ex.Message}", ct);
        }
        finally
        {
            await TitleOfPasswordToAdd.UpdateAsync(_ => "", ct);
            await PasswordToAdd.UpdateAsync(_ => "", ct);
        }
    }

    public async Task DeletePassword(PasswordPreview pwd, CancellationToken ct)
    {
        try
        {
            await SearchTerm.UpdateAsync(_ => "", ct);
            await Response.UpdateAsync(_ => "", ct);
            await DBService.DeletePassword(pwd.Id, ct);
            await Passwords.RemoveAllAsync(match: p => p.Id == pwd.Id, ct: ct);
            await Response.UpdateAsync(_ => $"Password '{pwd.Title}' deleted", ct);
        }
        catch (Exception ex)
        {
            await Response.UpdateAsync(_ => $"Error: {ex.Message}", ct);
        }
    }
}
