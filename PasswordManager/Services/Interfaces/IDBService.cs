namespace PasswordManager.Services.Interfaces;

public interface IDBService
{
    ValueTask CreateDB(
        byte[] masterPasswordHash,
        byte[] verSalt,
        byte[] encSalt,
        CancellationToken ct
    );
    ValueTask<Dictionary<string, byte[]>> GetPasswordHashAndSalt(CancellationToken ct);
    ValueTask<IImmutableList<Password>> GetPasswords(CancellationToken ct);
}
