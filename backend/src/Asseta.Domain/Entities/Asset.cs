using Asseta.Domain.Common;
using Asseta.Domain.Enums;

namespace Asseta.Domain.Entities;

public class Asset : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public AssetType Type { get; private set; }
    public decimal EstimatedValue { get; private set; }
    public string EncryptedVaultData { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }

    private Asset() { }

    public Asset(string name, string description, AssetType type, decimal estimatedValue, string encryptedVaultData, Guid ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Asset name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        Type = type;
        EstimatedValue = estimatedValue;
        EncryptedVaultData = encryptedVaultData;
        OwnerId = ownerId;
    }

    public void UpdateDetails(string name, string description, decimal estimatedValue)
    {
        Name = name;
        Description = description;
        EstimatedValue = estimatedValue;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
