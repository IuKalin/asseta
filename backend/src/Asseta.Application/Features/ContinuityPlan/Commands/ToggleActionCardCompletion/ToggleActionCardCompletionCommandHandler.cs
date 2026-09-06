using System.Text.Json;
using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityPlan.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityPlan.Commands.ToggleActionCardCompletion;

public class ToggleActionCardCompletionCommandHandler : IRequestHandler<ToggleActionCardCompletionCommand, ContinuityPlanCardItemDto>
{
    private readonly IAssetaDbContext _context;

    public ToggleActionCardCompletionCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ContinuityPlanCardItemDto> Handle(ToggleActionCardCompletionCommand request, CancellationToken cancellationToken)
    {
        var card = await _context.ActionCards
            .Include(c => c.Category)
            .Include(c => c.Steps)
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == request.CardId && c.OwnerId == request.OwnerId && !c.IsDeleted, cancellationToken);

        if (card == null)
            throw new NotFoundException(nameof(ActionCard), request.CardId);

        if (card.RowVersion != request.RowVersion)
            throw new ConcurrencyException($"The action card was modified by another operation. Expected {request.RowVersion}, but found {card.RowVersion}.");

        card.ToggleCompletion();

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            card.Id,
            "ACTION_CARD_COMPLETION_TOGGLED",
            JsonSerializer.Serialize(new
            {
                IsCompleted = card.IsCompleted,
                CardTitle = card.Title
            }),
            null,
            Guid.NewGuid()
        );
        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        string? delegateName = null;
        string? delegatePhone = null;
        if (card.AssignedTrustedPersonId.HasValue)
        {
            var tp = await _context.TrustedPeople
                .FirstOrDefaultAsync(p => p.Id == card.AssignedTrustedPersonId.Value && !p.IsDeleted, cancellationToken);
            if (tp != null)
            {
                delegateName = string.IsNullOrWhiteSpace(tp.Relationship)
                    ? tp.FullName
                    : $"{tp.FullName} ({tp.Relationship})";
                delegatePhone = tp.PhoneNumber;
            }
        }

        return new ContinuityPlanCardItemDto
        {
            Id = card.Id,
            CategoryId = card.CategoryId,
            CategoryName = card.Category?.NameVi ?? "Khác",
            CategoryIcon = card.Category?.Icon ?? "folder",
            Title = card.Title,
            Summary = card.Summary,
            Urgency = card.Urgency,
            Priority = card.Priority,
            AssignedTrustedPersonId = card.AssignedTrustedPersonId,
            AssignedTrustedPersonName = delegateName,
            AssignedTrustedPersonPhone = delegatePhone,
            DocumentLocationHint = card.DocumentLocationHint,
            HasDocumentLocation = !string.IsNullOrWhiteSpace(card.DocumentLocationHint) || !string.IsNullOrWhiteSpace(card.DigitalStorageLink),
            HasStageGap = PlanReadinessCalculator.HasStageGap(card),
            IsCompleted = card.IsCompleted,
            StepsCount = card.Steps.Count(s => !s.IsDeleted),
            ContactsCount = card.Contacts.Count(cnt => !cnt.IsDeleted),
            RowVersion = card.RowVersion
        };
    }
}
