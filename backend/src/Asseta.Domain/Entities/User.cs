using Asseta.Domain.Common;
using Asseta.Domain.Enums;

namespace Asseta.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string MasterKeyVerifier { get; private set; } = string.Empty;
    public string EncryptionSalt { get; private set; } = string.Empty;
    public UserStatus Status { get; private set; } = UserStatus.ACTIVE;

    private readonly List<UserRefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<UserRefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() { } // EF Core

    public User(
        string email,
        string passwordHash,
        string fullName,
        string masterKeyVerifier,
        string encryptionSalt,
        string? phoneNumber = null,
        Guid? id = null)
    {
        if (id.HasValue && id.Value != Guid.Empty)
        {
            Id = id.Value;
        }
        Email = email.ToLowerInvariant().Trim();
        PasswordHash = passwordHash;
        FullName = fullName.Trim();
        MasterKeyVerifier = masterKeyVerifier;
        EncryptionSalt = encryptionSalt;
        PhoneNumber = phoneNumber?.Trim();
        Status = UserStatus.ACTIVE;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateProfile(string fullName, string? phoneNumber)
    {
        FullName = fullName.Trim();
        PhoneNumber = phoneNumber?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void LockAccount()
    {
        Status = UserStatus.LOCKED;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UnlockAccount()
    {
        Status = UserStatus.ACTIVE;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
