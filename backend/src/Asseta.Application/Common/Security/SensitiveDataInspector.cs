using System.Text.RegularExpressions;
using Asseta.Application.Common.Exceptions;

namespace Asseta.Application.Common.Security;

public static class SensitiveDataInspector
{
    // Regex matches typical credit card numbers (Visa, MC, Amex, Discover: 13 to 19 digits)
    private static readonly Regex CreditCardPattern = new(
        @"\b(?:\d[ -]*?){13,19}\b",
        RegexOptions.Compiled);

    // Regex matches Ethereum / Bitcoin private keys, hex strings (64 hex characters), or PEM private keys
    private static readonly Regex PrivateKeyPattern = new(
        @"(-----BEGIN (?:RSA |EC )?PRIVATE KEY-----|\b0x[a-fA-F0-9]{64}\b|\b[a-fA-F0-9]{64}\b)",
        RegexOptions.Compiled);

    public static void EnsureSafe(string? text, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        if (CreditCardPattern.IsMatch(text))
        {
            // Verify digits count
            var digitsOnly = Regex.Replace(text, @"\D", "");
            if (digitsOnly.Length >= 13 && digitsOnly.Length <= 19)
            {
                throw new SensitiveDataDetectedException(fieldName, "CreditCardNumber");
            }
        }

        if (PrivateKeyPattern.IsMatch(text))
        {
            throw new SensitiveDataDetectedException(fieldName, "CryptographicPrivateKey");
        }
    }
}
