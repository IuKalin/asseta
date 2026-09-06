using Asseta.Domain.Common;

namespace Asseta.Domain.Entities;

public class UserRefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsRevoked { get; private set; }

    private UserRefreshToken() { } // EF Core

    public UserRefreshToken(Guid userId, string tokenHash, DateTime expiresAtUtc)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        IsRevoked = false;
    }

    public void Revoke()
    {
        IsRevoked = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public bool IsActive => !IsRevoked && !IsDeleted && ExpiresAtUtc > DateTime.UtcNow;
}
