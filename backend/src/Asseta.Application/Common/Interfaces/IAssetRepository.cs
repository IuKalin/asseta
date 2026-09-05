using Asseta.Domain.Entities;

namespace Asseta.Application.Common.Interfaces;

public interface IAssetRepository
{
    Task<IEnumerable<Asset>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Asset asset, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
