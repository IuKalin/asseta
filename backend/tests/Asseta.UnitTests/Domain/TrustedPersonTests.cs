using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Xunit;

namespace Asseta.UnitTests.Domain;

public class TrustedPersonTests
{
    [Fact]
    public void Constructor_WithValidArguments_CreatesInstanceSuccessfully()
    {
        var ownerId = Guid.NewGuid();
        var person = new TrustedPerson(
            "Nguyễn Văn B",
            "b@example.com",
            "0901234567",
            "Luật sư",
            2,
            ownerId,
            "Phụ trách pháp lý",
            TrustedPersonStatus.Invited);

        Assert.Equal("Nguyễn Văn B", person.FullName);
        Assert.Equal("b@example.com", person.Email);
        Assert.Equal("0901234567", person.PhoneNumber);
        Assert.Equal("Luật sư", person.Relationship);
        Assert.Equal(2, person.TrustLevel);
        Assert.Equal("Phụ trách pháp lý", person.RoleDescription);
        Assert.Equal(TrustedPersonStatus.Invited, person.Status);
        Assert.Equal(1, person.RowVersion);
        Assert.Null(person.DelegateUserId);
        Assert.Empty(person.PairingCodes);
        Assert.Empty(person.Permissions);
    }

    [Fact]
    public void MarkAsPaired_UpdatesDelegateUserIdAndStatusToActive()
    {
        var ownerId = Guid.NewGuid();
        var delegateUserId = Guid.NewGuid();
        var person = new TrustedPerson("Trần C", "c@example.com", "0912345678", "Vợ", 3, ownerId);

        person.MarkAsPaired(delegateUserId);

        Assert.Equal(delegateUserId, person.DelegateUserId);
        Assert.Equal(TrustedPersonStatus.Active, person.Status);
        Assert.Equal(2, person.RowVersion);
        Assert.NotNull(person.UpdatedAtUtc);
    }

    [Fact]
    public void Revoke_SetsStatusToRevoked_AndClearsPermissions()
    {
        var person = new TrustedPerson("Lê D", "d@example.com", "0987654321", "CFO", 2, Guid.NewGuid());
        var permissions = new List<TrustedPersonPermission>
        {
            TrustedPersonPermission.CreateForCategory(person.Id, Guid.NewGuid())
        };
        person.SetPermissions(permissions);
        Assert.Single(person.Permissions);

        person.Revoke();

        Assert.Equal(TrustedPersonStatus.Revoked, person.Status);
        Assert.Empty(person.Permissions);
    }

    [Fact]
    public void UpdateProfile_UpdatesFieldsAndIncrementsRowVersion()
    {
        var person = new TrustedPerson("Nguyễn E", "e@example.com", "0900000000", "Bạn thân", 1, Guid.NewGuid());

        person.UpdateProfile("Nguyễn E Updated", "e.updated@example.com", "0911111111", "Đồng sáng lập", "Quản lý kinh doanh", 2);

        Assert.Equal("Nguyễn E Updated", person.FullName);
        Assert.Equal("e.updated@example.com", person.Email);
        Assert.Equal("0911111111", person.PhoneNumber);
        Assert.Equal("Đồng sáng lập", person.Relationship);
        Assert.Equal("Quản lý kinh doanh", person.RoleDescription);
        Assert.Equal(2, person.TrustLevel);
        Assert.Equal(2, person.RowVersion);
    }

    [Fact]
    public void AddPairingCode_InvalidatesPreviousUnusedCodes()
    {
        var person = new TrustedPerson("Hoàng F", "f@example.com", "0922222222", "Kế toán", 2, Guid.NewGuid());
        var code1 = new TrustedPersonPairingCode(person.Id, "hash1", "salt1", DateTime.UtcNow.AddHours(48));
        var code2 = new TrustedPersonPairingCode(person.Id, "hash2", "salt2", DateTime.UtcNow.AddHours(48));

        person.AddPairingCode(code1);
        person.AddPairingCode(code2);

        Assert.Equal(2, person.PairingCodes.Count);
        Assert.True(code1.IsUsed);
        Assert.True(code1.IsDeleted);
        Assert.False(code2.IsUsed);
    }

    [Fact]
    public void PairingCode_RecordFailedAttempts_LocksOutAfter3Attempts()
    {
        var now = DateTime.UtcNow;
        var pairingCode = new TrustedPersonPairingCode(Guid.NewGuid(), "hash", "salt", now.AddHours(48));

        Assert.False(pairingCode.IsLockedOut(now));

        pairingCode.RecordFailedAttempt(now);
        pairingCode.RecordFailedAttempt(now);
        Assert.False(pairingCode.IsLockedOut(now));

        pairingCode.RecordFailedAttempt(now); // 3rd attempt
        Assert.True(pairingCode.IsLockedOut(now));
        Assert.True(pairingCode.IsLockedOut(now.AddMinutes(10)));
        Assert.False(pairingCode.IsLockedOut(now.AddMinutes(16)));
    }

    [Fact]
    public void PairingCode_IsExpired_ReturnsTrueWhenPastExpiration()
    {
        var now = DateTime.UtcNow;
        var pairingCode = new TrustedPersonPairingCode(Guid.NewGuid(), "hash", "salt", now.AddHours(48));

        Assert.False(pairingCode.IsExpired(now.AddHours(24)));
        Assert.True(pairingCode.IsExpired(now.AddHours(49)));
    }
}
