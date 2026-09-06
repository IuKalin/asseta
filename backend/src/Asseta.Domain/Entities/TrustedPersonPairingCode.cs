using Asseta.Domain.Common;

namespace Asseta.Domain.Entities;

public class TrustedPersonPairingCode : BaseEntity
{
    public Guid TrustedPersonId { get; private set; }
    public string CodeHash { get; private set; } = string.Empty;
    public string Salt { get; private set; } = string.Empty;
    public int FailedAttempts { get; private set; } = 0;
    public DateTime? LockoutUntil { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; } = false;
    public DateTime? UsedAt { get; private set; }

    // Navigation property
    public TrustedPerson? TrustedPerson { get; private set; }

    private TrustedPersonPairingCode() { }

    public TrustedPersonPairingCode(Guid trustedPersonId, string codeHash, string salt, DateTime expiresAt)
    {
        TrustedPersonId = trustedPersonId;
        CodeHash = codeHash;
        Salt = salt;
        ExpiresAt = expiresAt;
        FailedAttempts = 0;
        IsUsed = false;
    }

    public bool IsExpired(DateTime now) => now >= ExpiresAt;

    public bool IsLockedOut(DateTime now) => LockoutUntil.HasValue && now < LockoutUntil.Value;

    public void RecordFailedAttempt(DateTime now, int maxAttempts = 3, int lockoutMinutes = 15)
    {
        FailedAttempts++;
        UpdatedAtUtc = now;
        if (FailedAttempts >= maxAttempts)
        {
            LockoutUntil = now.AddMinutes(lockoutMinutes);
        }
    }

    public void ResetAttempts()
    {
        FailedAttempts = 0;
        LockoutUntil = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsUsed(DateTime now)
    {
        IsUsed = true;
        UsedAt = now;
        UpdatedAtUtc = now;
    }

    public void Invalidate()
    {
        IsUsed = true;
        MarkDeleted();
    }
}
