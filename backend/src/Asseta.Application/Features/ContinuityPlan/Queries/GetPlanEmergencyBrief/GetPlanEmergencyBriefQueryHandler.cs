using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityPlan.DTOs;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityPlan.Queries.GetPlanEmergencyBrief;

public class GetPlanEmergencyBriefQueryHandler : IRequestHandler<GetPlanEmergencyBriefQuery, OfflineEmergencyBriefDto>
{
    private readonly IAssetaDbContext _context;

    public GetPlanEmergencyBriefQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<OfflineEmergencyBriefDto> Handle(GetPlanEmergencyBriefQuery request, CancellationToken cancellationToken)
    {
        var cards = await _context.ActionCards
            .Include(c => c.Steps)
            .Include(c => c.Contacts)
            .Where(c => c.OwnerId == request.OwnerId && !c.IsDeleted)
            .ToListAsync(cancellationToken);

        var delegates = await _context.TrustedPeople
            .Where(tp => tp.OwnerId == request.OwnerId && !tp.IsDeleted)
            .ToListAsync(cancellationToken);

        var delegateDict = delegates.ToDictionary(tp => tp.Id);

        var brief = new OfflineEmergencyBriefDto
        {
            OwnerId = request.OwnerId,
            GeneratedAtUtc = DateTime.UtcNow
        };

        var stageDefinitions = new (UrgencyStage Stage, string Name)[]
        {
            (UrgencyStage.IMMEDIATE, "Immediate Actions (NOW)"),
            (UrgencyStage.FIRST_72_HOURS, "First 72 Hours"),
            (UrgencyStage.FIRST_7_DAYS, "First 7 Days"),
            (UrgencyStage.LONGER_TERM, "Longer-Term Continuity")
        };

        var allPrimaryContacts = new List<EmergencyContactSummaryDto>();

        // Add trusted delegates to primary contacts directory
        foreach (var d in delegates)
        {
            allPrimaryContacts.Add(new EmergencyContactSummaryDto
            {
                Name = d.FullName,
                Phone = d.PhoneNumber,
                RoleOrRelationship = $"Người Ủy Thác ({d.Relationship})",
                RelatedCardTitle = "Kế hoạch tiếp quản khẩn cấp"
            });
        }

        foreach (var (stage, name) in stageDefinitions)
        {
            var stageCards = cards
                .Where(c => c.Urgency == stage)
                .OrderByDescending(c => c.Priority)
                .ThenBy(c => c.Title)
                .ToList();

            var stageDto = new EmergencyBriefStageDto
            {
                Stage = stage,
                StageName = name
            };

            foreach (var card in stageCards)
            {
                string? delName = null;
                string? delPhone = null;
                if (card.AssignedTrustedPersonId.HasValue && delegateDict.TryGetValue(card.AssignedTrustedPersonId.Value, out var tp))
                {
                    delName = string.IsNullOrWhiteSpace(tp.Relationship) ? tp.FullName : $"{tp.FullName} ({tp.Relationship})";
                    delPhone = tp.PhoneNumber;
                }

                var keySteps = card.Steps
                    .Where(s => !s.IsDeleted)
                    .OrderBy(s => s.StepOrder)
                    .Select(s => s.Instruction)
                    .ToList();

                var keyContacts = card.Contacts
                    .Where(cnt => !cnt.IsDeleted)
                    .Select(cnt => $"{cnt.ContactName} ({cnt.RelationshipOrRole}) - {cnt.PhoneNumber}")
                    .ToList();

                // Add active card contacts to directory
                foreach (var cnt in card.Contacts.Where(c => !c.IsDeleted))
                {
                    allPrimaryContacts.Add(new EmergencyContactSummaryDto
                    {
                        Name = cnt.ContactName,
                        Phone = cnt.PhoneNumber,
                        RoleOrRelationship = cnt.RelationshipOrRole,
                        RelatedCardTitle = card.Title
                    });
                }

                stageDto.ActionItems.Add(new EmergencyBriefCardDto
                {
                    Id = card.Id,
                    Title = card.Title,
                    Priority = card.Priority,
                    DelegateName = delName,
                    DelegatePhone = delPhone,
                    DocumentLocationHint = card.DocumentLocationHint,
                    KeySteps = keySteps,
                    KeyContacts = keyContacts
                });
            }

            brief.Stages.Add(stageDto);
        }

        // Deduplicate contacts by Name and Phone
        brief.PrimaryContacts = allPrimaryContacts
            .GroupBy(c => $"{c.Name.Trim().ToLower()}_{c.Phone?.Trim()}")
            .Select(g => g.First())
            .ToList();

        return brief;
    }
}
