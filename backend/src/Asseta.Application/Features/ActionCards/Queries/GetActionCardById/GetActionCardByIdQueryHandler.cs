using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Queries.GetActionCardById;

public class GetActionCardByIdQueryHandler : IRequestHandler<GetActionCardByIdQuery, ActionCardDto>
{
    private readonly IAssetaDbContext _context;

    public GetActionCardByIdQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActionCardDto> Handle(GetActionCardByIdQuery request, CancellationToken cancellationToken)
    {
        var card = await _context.ActionCards
            .Include(c => c.Category)
            .Include(c => c.ContinuityItem)
            .Include(c => c.Steps)
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == request.CardId && !c.IsDeleted, cancellationToken);

        if (card == null)
        {
            throw new NotFoundException(nameof(ActionCard), request.CardId);
        }

        if (card.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        return ActionCardDto.FromEntity(card);
    }
}
