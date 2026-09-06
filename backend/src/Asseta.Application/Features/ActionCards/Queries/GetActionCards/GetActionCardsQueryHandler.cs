using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ActionCards.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Queries.GetActionCards;

public class GetActionCardsQueryHandler : IRequestHandler<GetActionCardsQuery, IReadOnlyList<ActionCardDto>>
{
    private readonly IAssetaDbContext _context;

    public GetActionCardsQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ActionCardDto>> Handle(GetActionCardsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ActionCards
            .Include(c => c.Category)
            .Include(c => c.ContinuityItem)
            .Include(c => c.Steps)
            .Include(c => c.Contacts)
            .Where(c => c.OwnerId == request.OwnerId && !c.IsDeleted);

        if (request.Urgency.HasValue)
        {
            query = query.Where(c => c.Urgency == request.Urgency.Value);
        }

        if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
        {
            query = query.Where(c => c.CategoryId == request.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            var search = request.SearchQuery.Trim().ToLower();
            query = query.Where(c =>
                c.Title.ToLower().Contains(search) ||
                (c.Summary != null && c.Summary.ToLower().Contains(search)) ||
                (c.DocumentLocationHint != null && c.DocumentLocationHint.ToLower().Contains(search)));
        }

        var cards = await query
            .OrderBy(c => c.Urgency)
            .ThenByDescending(c => c.Priority)
            .ThenByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return cards
            .Select(c => ActionCardDto.FromEntity(c))
            .ToList();
    }
}
