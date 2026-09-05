using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Xunit;

namespace Asseta.UnitTests.Domain;

public class AssetTests
{
    [Fact]
    public void CreateAsset_ValidInput_ShouldInstantiate()
    {
        var ownerId = Guid.NewGuid();
        var asset = new Asset("Bank Account", "Savings", AssetType.Financial, 10000m, "enc_data", ownerId);

        Assert.Equal("Bank Account", asset.Name);
        Assert.Equal(AssetType.Financial, asset.Type);
        Assert.False(asset.IsDeleted);
    }

    [Fact]
    public void CreateAsset_EmptyName_ShouldThrowArgumentException()
    {
        var ownerId = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() => new Asset("", "Desc", AssetType.Digital, 100m, "data", ownerId));
    }
}
