namespace CareerOS.Domain.Interfaces;

public interface IEncryptionService
{
    string Encrypt(string plainText, byte[] key);
    string Decrypt(string cipherText, byte[] key);
    byte[] DeriveKey(string password, byte[] salt, int iterations = 100000, int keyLength = 32);
    string HashPassword(string password, byte[] salt, int iterations = 100000);
    bool VerifyPassword(string password, string storedHash, byte[] salt, int iterations = 100000);
    byte[] GenerateRandomBytes(int length);
}
