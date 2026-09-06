namespace Asseta.Domain.ValueObjects;

public record CipherBlobPayload
{
    public string CipherBlob { get; }
    public string Nonce { get; }
    public string AuthTag { get; }

    public CipherBlobPayload(string cipherBlob, string nonce, string authTag)
    {
        if (string.IsNullOrWhiteSpace(cipherBlob))
            throw new ArgumentException("CipherBlob cannot be empty.", nameof(cipherBlob));
        if (string.IsNullOrWhiteSpace(nonce))
            throw new ArgumentException("Nonce cannot be empty.", nameof(nonce));
        if (string.IsNullOrWhiteSpace(authTag))
            throw new ArgumentException("AuthTag cannot be empty.", nameof(authTag));

        CipherBlob = cipherBlob;
        Nonce = nonce;
        AuthTag = authTag;
    }
}
