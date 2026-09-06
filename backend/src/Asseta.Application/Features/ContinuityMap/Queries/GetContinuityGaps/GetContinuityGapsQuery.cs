using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityMap.Queries.GetContinuityGaps;

public record GetContinuityGapsQuery(Guid OwnerId) : IRequest<IReadOnlyList<ContinuityGapDto>>;

public class GetContinuityGapsQueryHandler : IRequestHandler<GetContinuityGapsQuery, IReadOnlyList<ContinuityGapDto>>
{
    private readonly IAssetaDbContext _context;

    public GetContinuityGapsQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ContinuityGapDto>> Handle(GetContinuityGapsQuery request, CancellationToken cancellationToken)
    {
        var activeItems = await _context.ContinuityItems
            .Include(i => i.Category)
            .Where(i => i.OwnerId == request.OwnerId)
            .ToListAsync(cancellationToken);

        var gaps = ReadinessScoreCalculator.DetectGaps(activeItems);

        return gaps.Select(g =>
        {
            var missing = new List<string>();
            if (!g.AssignedTrustedPersonId.HasValue)
                missing.Add("AssignedTrustedPersonId");
            if (string.IsNullOrWhiteSpace(g.DocumentLocationHint))
                missing.Add("DocumentLocationHint");

            return new ContinuityGapDto(
                g.Id,
                g.Name,
                g.Category?.Code ?? "",
                g.Priority.ToString(),
                missing);
        }).ToList();
    }
}
