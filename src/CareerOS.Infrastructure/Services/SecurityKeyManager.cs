using System.Security.Cryptography;
using CareerOS.Domain.Entities;
using CareerOS.Domain.Interfaces;
using CareerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CareerOS.Infrastructure.Services;

public class SecurityKeyManager : ISecurityKeyManager
{
    private const string KeyVerifierSetting = "MasterPasswordVerifier";
    private const string KeySaltSetting = "MasterPasswordSalt";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<SecurityKeyManager> _logger;

    private byte[]? _activeKey;
    private readonly object _lock = new();

    public SecurityKeyManager(
        IServiceScopeFactory scopeFactory,
        IEncryptionService encryptionService,
        ILogger<SecurityKeyManager> logger)
    {
        _scopeFactory = scopeFactory;
        _encryptionService = encryptionService;
        _logger = logger;
    }

    public async Task<bool> IsMasterPasswordConfiguredAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.SecuritySettings
            .AnyAsync(s => s.Key == KeyVerifierSetting, cancellationToken);
    }

    public bool IsVaultUnlocked()
    {
        lock (_lock)
        {
            return _activeKey != null && _activeKey.Length == 32;
        }
    }

    public async Task<bool> SetupMasterPasswordAsync(string masterPassword, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(masterPassword) || masterPassword.Length < 6)
        {
            throw new ArgumentException("Master password must be at least 6 characters long.", nameof(masterPassword));
        }

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        byte[] salt = _encryptionService.GenerateRandomBytes(32);
        string hash = _encryptionService.HashPassword(masterPassword, salt);
        byte[] derivedKey = _encryptionService.DeriveKey(masterPassword, salt);

        string saltBase64 = Convert.ToBase64String(salt);

        var existingVerifier = await context.SecuritySettings.FirstOrDefaultAsync(s => s.Key == KeyVerifierSetting, cancellationToken);
        if (existingVerifier != null)
        {
            existingVerifier.Value = hash;
            existingVerifier.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            context.SecuritySettings.Add(new SecuritySetting { Key = KeyVerifierSetting, Value = hash, UpdatedAt = DateTime.UtcNow });
        }

        var existingSalt = await context.SecuritySettings.FirstOrDefaultAsync(s => s.Key == KeySaltSetting, cancellationToken);
        if (existingSalt != null)
        {
            existingSalt.Value = saltBase64;
            existingSalt.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            context.SecuritySettings.Add(new SecuritySetting { Key = KeySaltSetting, Value = saltBase64, UpdatedAt = DateTime.UtcNow });
        }

        await context.SaveChangesAsync(cancellationToken);

        lock (_lock)
        {
            _activeKey = derivedKey;
        }

        _logger.LogInformation("Master security key initialized and vault unlocked successfully.");
        return true;
    }

    public async Task<bool> UnlockVaultAsync(string masterPassword, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(masterPassword))
            return false;

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var verifier = await context.SecuritySettings.FirstOrDefaultAsync(s => s.Key == KeyVerifierSetting, cancellationToken);
        var saltSetting = await context.SecuritySettings.FirstOrDefaultAsync(s => s.Key == KeySaltSetting, cancellationToken);

        if (verifier == null || saltSetting == null)
        {
            _logger.LogWarning("Cannot unlock security vault: master password is not yet configured.");
            return false;
        }

        byte[] salt = Convert.FromBase64String(saltSetting.Value);
        bool isValid = _encryptionService.VerifyPassword(masterPassword, verifier.Value, salt);

        if (!isValid)
        {
            _logger.LogWarning("Failed security vault unlock attempt: invalid master password.");
            return false;
        }

        byte[] derivedKey = _encryptionService.DeriveKey(masterPassword, salt);
        lock (_lock)
        {
            _activeKey = derivedKey;
        }

        _logger.LogInformation("Security vault unlocked successfully.");
        return true;
    }

    public void LockVault()
    {
        lock (_lock)
        {
            if (_activeKey != null)
            {
                CryptographicOperations.ZeroMemory(_activeKey);
                _activeKey = null;
            }
        }
        _logger.LogInformation("Security vault locked and active encryption keys flushed from memory.");
    }

    public byte[] GetActiveEncryptionKey()
    {
        lock (_lock)
        {
            if (_activeKey == null || _activeKey.Length != 32)
            {
                throw new InvalidOperationException("Security vault is locked. Please unlock the vault with your master password first.");
            }

            byte[] copy = new byte[32];
            Buffer.BlockCopy(_activeKey, 0, copy, 0, 32);
            return copy;
        }
    }
}
