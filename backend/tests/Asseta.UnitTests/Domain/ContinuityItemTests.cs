using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Domain.ValueObjects;
using Xunit;

namespace Asseta.UnitTests.Domain;

public class ContinuityItemTests
{
    [Fact]
    public void CreateItem_ValidInput_ShouldInstantiateWithInitialValues()
    {
        var ownerId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var item = new ContinuityItem(
            ownerId,
            categoryId,
            "Hợp đồng bảo hiểm AIA",
            PriorityLevel.CRITICAL,
            "Tủ hồ sơ tầng 2");

        Assert.Equal("Hợp đồng bảo hiểm AIA", item.Name);
        Assert.Equal(PriorityLevel.CRITICAL, item.Priority);
        Assert.Equal("Tủ hồ sơ tầng 2", item.DocumentLocationHint);
        Assert.False(item.IsDeleted);
        Assert.Equal(1, item.RowVersion);
        Assert.False(item.IsCompleted);
        Assert.False(item.HasConfidentialNotes);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateItem_EmptyName_ShouldThrowArgumentException(string invalidName)
    {
        var ownerId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            new ContinuityItem(ownerId, categoryId, invalidName, PriorityLevel.IMPORTANT));
    }

    [Fact]
    public void SetEncryptedNotes_ValidPayload_ShouldStoreCiphertextOnly()
    {
        var item = new ContinuityItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Khoản vay thế chấp *9988",
            PriorityLevel.CRITICAL);

        var payload = new CipherBlobPayload("U2FsdGVkX1+...encrypted...", "nonce123", "tag456");
        item.SetEncryptedNotes(payload);

        Assert.True(item.HasConfidentialNotes);
        Assert.Equal("U2FsdGVkX1+...encrypted...", item.CipherNotesBlob);
        Assert.Equal("nonce123", item.CipherNonce);
        Assert.Equal("tag456", item.CipherAuthTag);
    }

    [Fact]
    public void HasContinuityGap_CriticalItemWithoutTrustedPerson_ShouldReturnTrue()
    {
        var item = new ContinuityItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Khoản vay thế chấp *9988",
            PriorityLevel.CRITICAL,
            documentLocationHint: "Két sắt");

        // Missing AssignedTrustedPersonId
        Assert.True(item.HasContinuityGap);
    }

    [Fact]
    public void HasContinuityGap_CriticalItemWithoutDocumentLocation_ShouldReturnTrue()
    {
        var item = new ContinuityItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Khoản vay thế chấp *9988",
            PriorityLevel.CRITICAL,
            documentLocationHint: null,
            assignedTrustedPersonId: Guid.NewGuid());

        // Missing DocumentLocationHint
        Assert.True(item.HasContinuityGap);
    }

    [Fact]
    public void HasContinuityGap_CriticalItemFullyConfigured_ShouldReturnFalse()
    {
        var item = new ContinuityItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Khoản vay thế chấp *9988",
            PriorityLevel.CRITICAL,
            documentLocationHint: "Ngăn tủ số 3",
            assignedTrustedPersonId: Guid.NewGuid());

        Assert.False(item.HasContinuityGap);
    }

    [Fact]
    public void SoftDelete_ShouldSetIsDeletedAndIncrementRowVersion()
    {
        var item = new ContinuityItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Hợp đồng thuê căn hộ",
            PriorityLevel.LOW);

        item.SoftDelete();

        Assert.True(item.IsDeleted);
        Assert.NotNull(item.DeletedAtUtc);
        Assert.Equal(2, item.RowVersion);
    }
}
