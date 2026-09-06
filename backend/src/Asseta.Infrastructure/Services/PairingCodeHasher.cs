using System.Security.Cryptography;
using System.Text;
using Asseta.Application.Common.Interfaces;

namespace Asseta.Infrastructure.Services;

public class PairingCodeHasher : IPairingCodeHasher
{
    private const string AllowedChars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ"; // 31 ký tự tránh nhầm lẫn 0/O, 1/I
    private const int CodeLength = 6;
    private const int SaltByteSize = 16;

    public string GeneratePairingCode()
    {
        var chars = new char[CodeLength];
        var bytes = new byte[CodeLength];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        for (int i = 0; i < CodeLength; i++)
        {
            chars[i] = AllowedChars[bytes[i] % AllowedChars.Length];
        }

        return new string(chars);
    }

    public (string Hash, string Salt) HashPairingCode(string code)
    {
        var saltBytes = new byte[SaltByteSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        using var hmac = new HMACSHA256(saltBytes);
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(normalizedCode));

        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public bool VerifyPairingCode(string code, string hash, string salt)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(hash) || string.IsNullOrWhiteSpace(salt))
        {
            return false;
        }

        try
        {
            var saltBytes = Convert.FromBase64String(salt);
            var expectedHashBytes = Convert.FromBase64String(hash);

            var normalizedCode = code.Trim().ToUpperInvariant();
            using var hmac = new HMACSHA256(saltBytes);
            var actualHashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(normalizedCode));

            return CryptographicOperations.FixedTimeEquals(actualHashBytes, expectedHashBytes);
        }
        catch
        {
            return false;
        }
    }
}
