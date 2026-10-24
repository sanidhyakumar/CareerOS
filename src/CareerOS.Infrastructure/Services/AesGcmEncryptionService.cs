using System.Security.Cryptography;
using System.Text;
using CareerOS.Domain.Interfaces;

namespace CareerOS.Infrastructure.Services;

public class AesGcmEncryptionService : IEncryptionService
{
    private const int NonceSize = 12; // 96-bit nonce standard for GCM
    private const int TagSize = 16;   // 128-bit authentication tag

    public string Encrypt(string plainText, byte[] key)
    {
        if (plainText == null) return string.Empty;
        if (key == null || key.Length != 32)
            throw new ArgumentException("Key must be 32 bytes (256-bit) for AES-256.", nameof(key));

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] nonce = GenerateRandomBytes(NonceSize);
        byte[] tag = new byte[TagSize];
        byte[] cipherBytes = new byte[plainBytes.Length];

        using (var aesGcm = new AesGcm(key, TagSize))
        {
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);
        }

        // Pack format: [12 bytes nonce][16 bytes tag][N bytes ciphertext]
        byte[] payload = new byte[NonceSize + TagSize + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, payload, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, payload, NonceSize, TagSize);
        Buffer.BlockCopy(cipherBytes, 0, payload, NonceSize + TagSize, cipherBytes.Length);

        return Convert.ToBase64String(payload);
    }

    public string Decrypt(string cipherText, byte[] key)
    {
        if (string.IsNullOrWhiteSpace(cipherText)) return string.Empty;
        if (key == null || key.Length != 32)
            throw new ArgumentException("Key must be 32 bytes (256-bit) for AES-256.", nameof(key));

        byte[] payload = Convert.FromBase64String(cipherText);
        if (payload.Length < NonceSize + TagSize)
            throw new CryptographicException("Ciphertext payload is truncated or invalid.");

        byte[] nonce = new byte[NonceSize];
        byte[] tag = new byte[TagSize];
        int cipherLength = payload.Length - NonceSize - TagSize;
        byte[] cipherBytes = new byte[cipherLength];
        byte[] plainBytes = new byte[cipherLength];

        Buffer.BlockCopy(payload, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(payload, NonceSize, tag, 0, TagSize);
        Buffer.BlockCopy(payload, NonceSize + TagSize, cipherBytes, 0, cipherLength);

        using (var aesGcm = new AesGcm(key, TagSize))
        {
            aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);
        }

        return Encoding.UTF8.GetString(plainBytes);
    }

    public byte[] DeriveKey(string password, byte[] salt, int iterations = 100000, int keyLength = 32)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            keyLength
        );
    }

    public string HashPassword(string password, byte[] salt, int iterations = 100000)
    {
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            32
        );
        return Convert.ToBase64String(hash);
    }

    public bool VerifyPassword(string password, string storedHash, byte[] salt, int iterations = 100000)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        byte[] expectedHash = Convert.FromBase64String(storedHash);
        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            32
        );

        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }

    public byte[] GenerateRandomBytes(int length)
    {
        return RandomNumberGenerator.GetBytes(length);
    }
}

