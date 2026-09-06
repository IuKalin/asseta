using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityPlan.DTOs;
using Asseta.Domain.Enums;
using Asseta.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityPlan.Queries.GetContinuityPlan;

public class GetContinuityPlanQueryHandler : IRequestHandler<GetContinuityPlanQuery, ContinuityPlanDto>
{
    private readonly IAssetaDbContext _context;

    public GetContinuityPlanQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ContinuityPlanDto> Handle(GetContinuityPlanQuery request, CancellationToken cancellationToken)
    {
        var cards = await _context.ActionCards
            .Include(c => c.Category)
            .Include(c => c.Steps)
            .Include(c => c.Contacts)
            .Where(c => c.OwnerId == request.OwnerId && !c.IsDeleted)
            .ToListAsync(cancellationToken);

        var delegates = await _context.TrustedPeople
            .Where(tp => tp.OwnerId == request.OwnerId && !tp.IsDeleted)
            .ToListAsync(cancellationToken);

        var delegateDict = delegates.ToDictionary(tp => tp.Id);
        var readinessReport = PlanReadinessCalculator.CalculatePlanReadiness(cards);

        var stageDefinitions = new (UrgencyStage Stage, string Name, string Description)[]
        {
            (UrgencyStage.IMMEDIATE, "Immediate Actions (NOW)", "Những việc phải xử lý ngay trong 24 giờ đầu khi xảy ra biến cố"),
            (UrgencyStage.FIRST_72_HOURS, "First 72 Hours", "Ổn định hoạt động và giải quyết nghĩa vụ cấp thiết (24–72 giờ)"),
            (UrgencyStage.FIRST_7_DAYS, "First 7 Days", "Làm việc với các tổ chức tài chính, đối tác và thủ tục hành chính (3–7 ngày)"),
            (UrgencyStage.LONGER_TERM, "Longer-Term Continuity", "Quản lý và tiếp quản dài hạn sau 7 ngày đến 30 ngày")
        };

        var planDto = new ContinuityPlanDto
        {
            OwnerId = request.OwnerId,
            TotalCardsCount = cards.Count,
            CompletedCardsCount = cards.Count(c => c.IsCompleted),
            GapsCount = readinessReport.TotalGapsCount,
            DelegateCoveragePercentage = readinessReport.DelegateCoveragePercentage,
            DocumentReadinessPercentage = readinessReport.DocumentReadinessPercentage,
            OverallPlanReadinessScore = readinessReport.OverallScore,
            HasSinglePointOfFailureRisk = readinessReport.HasSinglePointOfFailureRisk
        };

        if (readinessReport.HasSinglePointOfFailureRisk && readinessReport.DominantDelegateId.HasValue)
        {
            if (delegateDict.TryGetValue(readinessReport.DominantDelegateId.Value, out var domDelegate))
            {
                planDto.SinglePointOfFailureWarning =
                    $"Cảnh báo điểm nghẽn: Người ủy thác '{domDelegate.FullName}' đang đảm nhận {readinessReport.DominantDelegateRatio}% việc khẩn cấp (Immediate/72h). Hãy cân nhắc chia sẻ bớt trách nhiệm để giảm thiểu rủi ro.";
            }
        }

        foreach (var (stage, name, description) in stageDefinitions)
        {
            var stageCards = cards
                .Where(c => c.Urgency == stage)
                .OrderByDescending(c => c.Priority)
                .ThenBy(c => c.Title)
                .ToList();

            var cardDtos = stageCards.Select(c =>
            {
                string? personName = null;
                string? personPhone = null;

                if (c.AssignedTrustedPersonId.HasValue && delegateDict.TryGetValue(c.AssignedTrustedPersonId.Value, out var tp))
                {
                    personName = string.IsNullOrWhiteSpace(tp.Relationship)
                        ? tp.FullName
                        : $"{tp.FullName} ({tp.Relationship})";
                    personPhone = tp.PhoneNumber;
                }

                return new ContinuityPlanCardItemDto
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
                    AssignedTrustedPersonName = personName,
                    AssignedTrustedPersonPhone = personPhone,
                    DocumentLocationHint = c.DocumentLocationHint,
                    HasDocumentLocation = !string.IsNullOrWhiteSpace(c.DocumentLocationHint) || !string.IsNullOrWhiteSpace(c.DigitalStorageLink),
                    HasStageGap = PlanReadinessCalculator.HasStageGap(c),
                    IsCompleted = c.IsCompleted,
                    StepsCount = c.Steps.Count(s => !s.IsDeleted),
                    ContactsCount = c.Contacts.Count(cnt => !cnt.IsDeleted),
                    RowVersion = c.RowVersion
                };
            }).ToList();

            planDto.Stages.Add(new ContinuityPlanStageDto
            {
                Stage = stage,
                StageName = name,
                StageDescription = description,
                TotalCardsCount = cardDtos.Count,
                CompletedCardsCount = cardDtos.Count(c => c.IsCompleted),
                GapCardsCount = cardDtos.Count(c => c.HasStageGap),
                Cards = cardDtos
            });
        }

        return planDto;
    }
}
