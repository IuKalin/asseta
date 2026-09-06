using System.Text.Json;
using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Commands.CreateActionCardFromItem;

public class CreateActionCardFromItemCommandHandler : IRequestHandler<CreateActionCardFromItemCommand, ActionCardDto>
{
    private readonly IAssetaDbContext _context;

    public CreateActionCardFromItemCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActionCardDto> Handle(CreateActionCardFromItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ContinuityItems
            .Include(i => i.Category)
            .FirstOrDefaultAsync(i => i.Id == request.ContinuityItemId && !i.IsDeleted, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException(nameof(ContinuityItem), request.ContinuityItemId);
        }

        if (item.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        // Determine Urgency and Priority defaults
        UrgencyStage urgency = request.Urgency ?? UrgencyStage.FIRST_72_HOURS;
        PriorityLevel priority = request.Priority ?? item.Priority;

        ActionCardTemplate? template = null;
        if (!string.IsNullOrWhiteSpace(request.TemplateCode))
        {
            template = await _context.ActionCardTemplates
                .FirstOrDefaultAsync(t => t.TemplateCode == request.TemplateCode.Trim().ToUpperInvariant() && !t.IsDeleted, cancellationToken);

            if (template != null)
            {
                if (!request.Urgency.HasValue && Enum.TryParse<UrgencyStage>(template.DefaultUrgency, true, out var parsedUrgency))
                {
                    urgency = parsedUrgency;
                }

                if (!request.Priority.HasValue && Enum.TryParse<PriorityLevel>(template.DefaultPriority, true, out var parsedPriority))
                {
                    priority = parsedPriority;
                }
            }
        }

        var card = new ActionCard(
            Guid.NewGuid(),
            request.OwnerId,
            item.CategoryId,
            item.Name,
            urgency,
            priority,
            continuityItemId: item.Id,
            summary: request.Summary,
            assignedTrustedPersonId: item.AssignedTrustedPersonId,
            documentLocationHint: item.DocumentLocationHint,
            digitalStorageLink: null,
            cipherInstructions: null);

        // Apply template steps if template was provided
        if (template != null && !string.IsNullOrWhiteSpace(template.SuggestedStepsJson))
        {
            try
            {
                var steps = JsonSerializer.Deserialize<List<TemplateStepDto>>(template.SuggestedStepsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (steps != null)
                {
                    foreach (var s in steps.OrderBy(s => s.StepOrder))
                    {
                        card.AddStep(s.Instruction, s.EstimatedDuration);
                    }
                }
            }
            catch
            {
                // Fallback gracefully if json parsing fails
            }
        }

        // Apply template suggested roles as contact placeholders if template provided
        if (template != null && !string.IsNullOrWhiteSpace(template.SuggestedRolesJson))
        {
            try
            {
                var roles = JsonSerializer.Deserialize<List<string>>(template.SuggestedRolesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (roles != null)
                {
                    foreach (var role in roles.Take(5))
                    {
                        card.AddContact("[Chưa chỉ định]", role);
                    }
                }
            }
            catch
            {
                // Fallback gracefully
            }
        }

        // Link item to action card
        item.LinkActionCard(card.Id);

        _context.ActionCards.Add(card);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            card.Id,
            "ACTION_CARD_CREATED_FROM_ITEM",
            $"{{\"title\":\"{card.Title}\",\"continuityItemId\":\"{item.Id}\",\"templateCode\":\"{request.TemplateCode}\"}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return ActionCardDto.FromEntity(card, item.Category?.Code ?? "", item.Category?.NameVi ?? "", item.Name);
    }
}
