namespace CareerOS.Domain.Interfaces;

public interface ISecurityKeyManager
{
    Task<bool> IsMasterPasswordConfiguredAsync(CancellationToken cancellationToken = default);
    bool IsVaultUnlocked();
    Task<bool> SetupMasterPasswordAsync(string masterPassword, CancellationToken cancellationToken = default);
    Task<bool> UnlockVaultAsync(string masterPassword, CancellationToken cancellationToken = default);
    void LockVault();
    byte[] GetActiveEncryptionKey();
}
