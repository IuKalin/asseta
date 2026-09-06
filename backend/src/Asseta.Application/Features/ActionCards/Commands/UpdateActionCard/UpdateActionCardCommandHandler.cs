using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Commands.UpdateActionCard;

public class UpdateActionCardCommandHandler : IRequestHandler<UpdateActionCardCommand, ActionCardDto>
{
    private readonly IAssetaDbContext _context;

    public UpdateActionCardCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActionCardDto> Handle(UpdateActionCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _context.ActionCards
            .Include(c => c.Category)
            .Include(c => c.ContinuityItem)
            .Include(c => c.Steps)
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (card == null)
        {
            throw new NotFoundException(nameof(ActionCard), request.Id);
        }

        if (card.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        if (card.RowVersion != request.RowVersion)
        {
            throw new ConcurrencyException($"Card has been modified by another process. Expected version {request.RowVersion}, but found {card.RowVersion}.");
        }

        CipherBlobPayload? encryptedInstructions = null;
        if (!string.IsNullOrWhiteSpace(request.CipherInstructionsBlob) &&
            !string.IsNullOrWhiteSpace(request.CipherNonce) &&
            !string.IsNullOrWhiteSpace(request.CipherAuthTag))
        {
            encryptedInstructions = new CipherBlobPayload(
                request.CipherInstructionsBlob,
                request.CipherNonce,
                request.CipherAuthTag);
        }

        card.Update(
            request.Title,
            request.Urgency,
            request.Priority,
            request.Summary,
            request.AssignedTrustedPersonId,
            request.DocumentLocationHint,
            request.DigitalStorageLink,
            encryptedInstructions);

        // Sync with linked ContinuityItem if present
        if (card.ContinuityItemId.HasValue && card.ContinuityItem != null && !card.ContinuityItem.IsDeleted)
        {
            if (card.AssignedTrustedPersonId.HasValue || !string.IsNullOrWhiteSpace(card.DocumentLocationHint))
            {
                card.ContinuityItem.UpdateDetails(
                    card.ContinuityItem.Name,
                    card.ContinuityItem.Priority,
                    card.DocumentLocationHint ?? card.ContinuityItem.DocumentLocationHint,
                    card.AssignedTrustedPersonId ?? card.ContinuityItem.AssignedTrustedPersonId);
            }
        }

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            card.Id,
            "ACTION_CARD_UPDATED",
            $"{{\"title\":\"{card.Title}\",\"urgency\":\"{card.Urgency}\",\"priority\":\"{card.Priority}\",\"newVersion\":{card.RowVersion}}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return ActionCardDto.FromEntity(card);
    }
}
