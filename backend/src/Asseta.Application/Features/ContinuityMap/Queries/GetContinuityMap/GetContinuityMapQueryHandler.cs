using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityMap.Queries.GetContinuityMap;

public class GetContinuityMapQueryHandler : IRequestHandler<GetContinuityMapQuery, ContinuityMapDto>
{
    private readonly IAssetaDbContext _context;

    public GetContinuityMapQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ContinuityMapDto> Handle(GetContinuityMapQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.ContinuityCategories
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);

        var activeItems = await _context.ContinuityItems
            .Include(i => i.Category)
            .Where(i => i.OwnerId == request.OwnerId)
            .OrderBy(i => i.SortOrder)
            .ThenByDescending(i => i.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var itemsByCategory = categories.ToDictionary(
            c => c.Id,
            c => activeItems.Where(i => i.CategoryId == c.Id).AsEnumerable());

        var categoryDtos = new List<ContinuityCategoryDto>();

        foreach (var category in categories)
        {
            var items = itemsByCategory[category.Id].ToList();
            var categoryScore = ReadinessScoreCalculator.CalculateCategoryScore(items);

            var itemDtos = items
                .Select(i => ContinuityItemDto.FromEntity(i, category.Code))
                .ToList();

            categoryDtos.Add(new ContinuityCategoryDto(
                category.Id,
                category.Code,
                category.NameVi,
                category.Icon,
                category.SortOrder,
                categoryScore.Value,
                itemDtos));
        }

        var overallScore = ReadinessScoreCalculator.CalculateOverallScore(itemsByCategory);
        var gaps = ReadinessScoreCalculator.DetectGaps(activeItems);

        var gapDtos = gaps.Select(g =>
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

        return new ContinuityMapDto(
            overallScore.Value,
            activeItems.Count,
            gapDtos.Count,
            categoryDtos,
            gapDtos);
    }
}
