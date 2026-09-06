using Asseta.Domain.Common;
using Asseta.Domain.Enums;

namespace Asseta.Domain.Entities;

public class TrustedPerson : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Relationship { get; private set; } = string.Empty;
    public string? RoleDescription { get; private set; }
    public int TrustLevel { get; private set; } = 1;
    public TrustedPersonStatus Status { get; private set; } = TrustedPersonStatus.Invited;
    public Guid OwnerId { get; private set; }
    public Guid? DelegateUserId { get; private set; }
    public int RowVersion { get; private set; } = 1;

    private readonly List<TrustedPersonPairingCode> _pairingCodes = new();
    public IReadOnlyCollection<TrustedPersonPairingCode> PairingCodes => _pairingCodes.AsReadOnly();

    private readonly List<TrustedPersonPermission> _permissions = new();
    public IReadOnlyCollection<TrustedPersonPermission> Permissions => _permissions.AsReadOnly();

    private TrustedPerson() { }

    public TrustedPerson(
        string fullName,
        string email,
        string phoneNumber,
        string relationship,
        int trustLevel,
        Guid ownerId,
        string? roleDescription = null,
        TrustedPersonStatus status = TrustedPersonStatus.Invited)
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        Relationship = relationship;
        TrustLevel = trustLevel;
        OwnerId = ownerId;
        RoleDescription = roleDescription;
        Status = status;
        RowVersion = 1;
    }

    public void UpdateProfile(
        string fullName,
        string email,
        string phoneNumber,
        string relationship,
        string? roleDescription,
        int trustLevel)
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        Relationship = relationship;
        RoleDescription = roleDescription;
        TrustLevel = trustLevel;
        UpdatedAtUtc = DateTime.UtcNow;
        RowVersion++;
    }

    public void UpdateTrustLevel(int trustLevel)
    {
        TrustLevel = trustLevel;
        UpdatedAtUtc = DateTime.UtcNow;
        RowVersion++;
    }

    public void MarkAsPaired(Guid delegateUserId)
    {
        DelegateUserId = delegateUserId;
        Status = TrustedPersonStatus.Active;
        UpdatedAtUtc = DateTime.UtcNow;
        RowVersion++;
    }

    public void Revoke()
    {
        Status = TrustedPersonStatus.Revoked;
        UpdatedAtUtc = DateTime.UtcNow;
        RowVersion++;
        _permissions.Clear();
    }

    public void Suspend()
    {
        Status = TrustedPersonStatus.Suspended;
        UpdatedAtUtc = DateTime.UtcNow;
        RowVersion++;
    }

    public void Activate()
    {
        Status = TrustedPersonStatus.Active;
        UpdatedAtUtc = DateTime.UtcNow;
        RowVersion++;
    }

    public void AddPairingCode(TrustedPersonPairingCode pairingCode)
    {
        // Vô hiệu hóa các mã cũ chưa sử dụng
        foreach (var existingCode in _pairingCodes.Where(c => !c.IsUsed && !c.IsDeleted))
        {
            existingCode.Invalidate();
        }

        _pairingCodes.Add(pairingCode);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetPermissions(IEnumerable<TrustedPersonPermission> newPermissions)
    {
        _permissions.Clear();
        _permissions.AddRange(newPermissions);
        UpdatedAtUtc = DateTime.UtcNow;
        RowVersion++;
    }

    public void SetRowVersion(int version)
    {
        RowVersion = version;
    }
}
