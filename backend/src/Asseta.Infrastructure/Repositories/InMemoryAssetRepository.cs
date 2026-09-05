using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;

namespace Asseta.Infrastructure.Repositories;

public class InMemoryAssetRepository : IAssetRepository
{
    private static readonly List<Asset> _assets = new()
    {
        new Asset("Real Estate Villa", "Primary family residence", AssetType.RealEstate, 500000m, "vault_encrypted_data_1", Guid.Parse("11111111-1111-1111-1111-111111111111")),
        new Asset("Global Equity Fund", "Stock brokerage investment", AssetType.Financial, 250000m, "vault_encrypted_data_2", Guid.Parse("11111111-1111-1111-1111-111111111111")),
        new Asset("Cold Storage Crypto Vault", "Bitcoin and Ethereum reserve", AssetType.Digital, 120000m, "vault_encrypted_data_3", Guid.Parse("11111111-1111-1111-1111-111111111111"))
    };

    public Task<IEnumerable<Asset>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        var result = _assets.Where(a => a.OwnerId == ownerId && !a.IsDeleted).AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = _assets.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
        return Task.FromResult(result);
    }

    public Task AddAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        _assets.Add(asset);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
