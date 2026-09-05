using Asseta.Application.Common.Interfaces;
using Asseta.Application.DTOs;

namespace Asseta.Application.Features.ContinuityMap.Services;

public class ContinuityMapService
{
    private readonly IAssetRepository _assetRepository;

    public ContinuityMapService(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<IEnumerable<AssetDto>> GetContinuityMapAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        var assets = await _assetRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        return assets.Select(a => new AssetDto(
            a.Id,
            a.Name,
            a.Description,
            a.Type,
            a.EstimatedValue,
            a.CreatedAtUtc
        ));
    }
}
