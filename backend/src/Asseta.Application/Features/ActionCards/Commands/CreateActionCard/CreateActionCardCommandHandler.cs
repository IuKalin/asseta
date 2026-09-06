using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Commands.CreateActionCard;

public class CreateActionCardCommandHandler : IRequestHandler<CreateActionCardCommand, ActionCardDto>
{
    private readonly IAssetaDbContext _context;

    public CreateActionCardCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActionCardDto> Handle(CreateActionCardCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.ContinuityCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException(nameof(ContinuityCategory), request.CategoryId);
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

        var card = new ActionCard(
            Guid.NewGuid(),
            request.OwnerId,
            request.CategoryId,
            request.Title,
            request.Urgency,
            request.Priority,
            continuityItemId: null,
            summary: request.Summary,
            assignedTrustedPersonId: request.AssignedTrustedPersonId,
            documentLocationHint: request.DocumentLocationHint,
            digitalStorageLink: request.DigitalStorageLink,
            cipherInstructions: encryptedInstructions);

        _context.ActionCards.Add(card);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            card.Id,
            "ACTION_CARD_CREATED",
            $"{{\"title\":\"{card.Title}\",\"urgency\":\"{card.Urgency}\",\"priority\":\"{card.Priority}\"}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return ActionCardDto.FromEntity(card, category.Code, category.NameVi);
    }
}
