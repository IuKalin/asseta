using System.Security.Cryptography;
using System.Text;

namespace Asseta.Application.Common.Security;

public static class MasterKeyGenerator
{
    private static readonly char[] KeyChars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ".ToCharArray();

    public static string GenerateMasterKey()
    {
        var sb = new StringBuilder("AK-");
        var randomBytes = new byte[16];
        RandomNumberGenerator.Fill(randomBytes);

        for (int i = 0; i < 16; i++)
        {
            if (i > 0 && i % 4 == 0)
            {
                sb.Append('-');
            }
            sb.Append(KeyChars[randomBytes[i] % KeyChars.Length]);
        }

        return sb.ToString();
    }

    public static string GenerateSaltHex()
    {
        var saltBytes = new byte[16];
        RandomNumberGenerator.Fill(saltBytes);
        return Convert.ToHexString(saltBytes).ToLowerInvariant();
    }

    public static string ComputeVerifier(string masterKey)
    {
        var bytes = Encoding.UTF8.GetBytes(masterKey);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
