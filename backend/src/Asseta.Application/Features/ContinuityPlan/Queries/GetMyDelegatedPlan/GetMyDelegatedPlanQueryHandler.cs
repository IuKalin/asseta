using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityPlan.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityPlan.Queries.GetMyDelegatedPlan;

public class GetMyDelegatedPlanQueryHandler : IRequestHandler<GetMyDelegatedPlanQuery, ContinuityPlanDto>
{
    private readonly IAssetaDbContext _context;

    public GetMyDelegatedPlanQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ContinuityPlanDto> Handle(GetMyDelegatedPlanQuery request, CancellationToken cancellationToken)
    {
        var delegations = await _context.TrustedPeople
            .Include(tp => tp.Permissions)
            .Where(tp => tp.DelegateUserId == request.DelegateUserId &&
                         tp.Status == TrustedPersonStatus.Active &&
                         !tp.IsDeleted)
            .ToListAsync(cancellationToken);

        var stageDefinitions = new (UrgencyStage Stage, string Name, string Description)[]
        {
            (UrgencyStage.IMMEDIATE, "Immediate Actions (NOW)", "Những việc phải xử lý ngay trong 24 giờ đầu khi xảy ra biến cố"),
            (UrgencyStage.FIRST_72_HOURS, "First 72 Hours", "Ổn định hoạt động và giải quyết nghĩa vụ cấp thiết (24–72 giờ)"),
            (UrgencyStage.FIRST_7_DAYS, "First 7 Days", "Làm việc với các tổ chức tài chính, đối tác và thủ tục hành chính (3–7 ngày)"),
            (UrgencyStage.LONGER_TERM, "Longer-Term Continuity", "Quản lý và tiếp quản dài hạn sau 7 ngày đến 30 ngày")
        };

        var planDto = new ContinuityPlanDto
        {
            OwnerId = request.DelegateUserId
        };

        if (delegations.Count == 0)
        {
            foreach (var (stage, name, description) in stageDefinitions)
            {
                planDto.Stages.Add(new ContinuityPlanStageDto
                {
                    Stage = stage,
                    StageName = name,
                    StageDescription = description,
                    Cards = new List<ContinuityPlanCardItemDto>()
                });
            }
            return planDto;
        }

        var visibleCards = new List<ActionCard>();

        foreach (var tp in delegations)
        {
            // Level 1: Notice Only -> Zero disclosure of cards in normal state
            if (tp.TrustLevel == 1)
                continue;

            var ownerCards = await _context.ActionCards
                .Include(c => c.Category)
                .Include(c => c.Steps)
                .Include(c => c.Contacts)
                .Where(c => c.OwnerId == tp.OwnerId && !c.IsDeleted)
                .ToListAsync(cancellationToken);

            // Level 3: Primary Delegate -> Can view all cards
            if (tp.TrustLevel == 3)
            {
                visibleCards.AddRange(ownerCards);
                continue;
            }

            // Level 2: Scoped Delegate -> Filter by assigned or permitted categories/cards
            var permittedCategoryIds = tp.Permissions
                .Where(p => p.PermissionType == PermissionType.Category && p.CanView && p.TargetCategoryId.HasValue)
                .Select(p => p.TargetCategoryId!.Value)
                .ToHashSet();

            var permittedCardIds = tp.Permissions
                .Where(p => p.PermissionType == PermissionType.ActionCard && p.CanView && p.TargetActionCardId.HasValue)
                .Select(p => p.TargetActionCardId!.Value)
                .ToHashSet();

            var scoped = ownerCards.Where(c =>
                (c.AssignedTrustedPersonId.HasValue && c.AssignedTrustedPersonId.Value == tp.Id) ||
                permittedCardIds.Contains(c.Id) ||
                permittedCategoryIds.Contains(c.CategoryId)
            ).ToList();

            visibleCards.AddRange(scoped);
        }

        // Deduplicate cards by Id
        var distinctCards = visibleCards
            .GroupBy(c => c.Id)
            .Select(g => g.First())
            .ToList();

        var readinessReport = PlanReadinessCalculator.CalculatePlanReadiness(distinctCards);
        planDto.TotalCardsCount = distinctCards.Count;
        planDto.CompletedCardsCount = distinctCards.Count(c => c.IsCompleted);
        planDto.GapsCount = readinessReport.TotalGapsCount;
        planDto.DelegateCoveragePercentage = readinessReport.DelegateCoveragePercentage;
        planDto.DocumentReadinessPercentage = readinessReport.DocumentReadinessPercentage;
        planDto.OverallPlanReadinessScore = readinessReport.OverallScore;

        foreach (var (stage, name, description) in stageDefinitions)
        {
            var stageCards = distinctCards
                .Where(c => c.Urgency == stage)
                .OrderByDescending(c => c.Priority)
                .ThenBy(c => c.Title)
                .Select(c => new ContinuityPlanCardItemDto
                {
                    Id = c.Id,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category?.NameVi ?? "Khác",
                    CategoryIcon = c.Category?.Icon ?? "folder",
                    Title = c.Title,
                    Summary = c.Summary,
                    Urgency = c.Urgency,
                    Priority = c.Priority,
                    AssignedTrustedPersonId = c.AssignedTrustedPersonId,
                    DocumentLocationHint = c.DocumentLocationHint,
                    HasDocumentLocation = !string.IsNullOrWhiteSpace(c.DocumentLocationHint) || !string.IsNullOrWhiteSpace(c.DigitalStorageLink),
                    HasStageGap = PlanReadinessCalculator.HasStageGap(c),
                    IsCompleted = c.IsCompleted,
                    StepsCount = c.Steps.Count(s => !s.IsDeleted),
                    ContactsCount = c.Contacts.Count(cnt => !cnt.IsDeleted),
                    RowVersion = c.RowVersion
                })
                .ToList();

            planDto.Stages.Add(new ContinuityPlanStageDto
            {
                Stage = stage,
                StageName = name,
                StageDescription = description,
                TotalCardsCount = stageCards.Count,
                CompletedCardsCount = stageCards.Count(c => c.IsCompleted),
                GapCardsCount = stageCards.Count(c => c.HasStageGap),
                Cards = stageCards
            });
        }

        return planDto;
    }
}
