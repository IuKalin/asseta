using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Commands.DeleteActionCard;

public class DeleteActionCardCommandHandler : IRequestHandler<DeleteActionCardCommand, bool>
{
    private readonly IAssetaDbContext _context;

    public DeleteActionCardCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteActionCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _context.ActionCards
            .Include(c => c.ContinuityItem)
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (card == null)
        {
            throw new NotFoundException(nameof(ActionCard), request.Id);
        }

        if (card.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        card.MarkDeleted();

        if (card.ContinuityItemId.HasValue && card.ContinuityItem != null)
        {
            card.ContinuityItem.UnlinkActionCard();
        }

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            card.Id,
            "ACTION_CARD_DELETED",
            $"{{\"title\":\"{card.Title}\"}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
