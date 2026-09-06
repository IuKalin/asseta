using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityPlan.DTOs;
using Asseta.Domain.Enums;
using Asseta.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityPlan.Queries.GetPlanReadinessAudit;

public class GetPlanReadinessAuditQueryHandler : IRequestHandler<GetPlanReadinessAuditQuery, PlanReadinessAuditDto>
{
    private readonly IAssetaDbContext _context;

    public GetPlanReadinessAuditQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<PlanReadinessAuditDto> Handle(GetPlanReadinessAuditQuery request, CancellationToken cancellationToken)
    {
        var cards = await _context.ActionCards
            .Where(c => c.OwnerId == request.OwnerId && !c.IsDeleted)
            .ToListAsync(cancellationToken);

        var delegates = await _context.TrustedPeople
            .Where(tp => tp.OwnerId == request.OwnerId && !tp.IsDeleted)
            .ToListAsync(cancellationToken);

        var delegateDict = delegates.ToDictionary(tp => tp.Id);
        var readinessReport = PlanReadinessCalculator.CalculatePlanReadiness(cards);

        var audit = new PlanReadinessAuditDto
        {
            OwnerId = request.OwnerId,
            OverallScore = readinessReport.OverallScore,
            DelegateCoveragePercentage = readinessReport.DelegateCoveragePercentage,
            DocumentReadinessPercentage = readinessReport.DocumentReadinessPercentage,
            TotalGapsCount = readinessReport.TotalGapsCount,
            HasSinglePointOfFailureRisk = readinessReport.HasSinglePointOfFailureRisk
        };

        if (readinessReport.HasSinglePointOfFailureRisk && readinessReport.DominantDelegateId.HasValue)
        {
            if (delegateDict.TryGetValue(readinessReport.DominantDelegateId.Value, out var tp))
            {
                audit.SinglePointOfFailureDetails =
                    $"Rủi ro điểm nghẽn đơn lẻ (SPoF): Người ủy thác '{tp.FullName}' hiện đang được chỉ định đảm nhận {readinessReport.DominantDelegateRatio}% tổng số thẻ khẩn cấp trong 72 giờ đầu. Nếu người này gặp sự cố hoặc không thể liên lạc, kế hoạch sẽ bị đình trệ.";
            }
        }

        var stageNames = new Dictionary<UrgencyStage, string>
        {
            { UrgencyStage.IMMEDIATE, "Immediate Actions (NOW)" },
            { UrgencyStage.FIRST_72_HOURS, "First 72 Hours" },
            { UrgencyStage.FIRST_7_DAYS, "First 7 Days" },
            { UrgencyStage.LONGER_TERM, "Longer-Term Continuity" }
        };

        foreach (var (stage, metrics) in readinessReport.StageMetrics)
        {
            audit.StageAudits.Add(new StageAuditSummaryDto
            {
                Stage = stage,
                StageName = stageNames.TryGetValue(stage, out var sName) ? sName : stage.ToString(),
                TotalCards = metrics.TotalCards,
                GapCards = metrics.GapCards,
                StageScore = metrics.StageScore
            });
        }

        int immediateGapsCount = 0;
        int docGapsCount = 0;
        int delegateGapsCount = 0;

        foreach (var card in cards)
        {
            if (PlanReadinessCalculator.HasStageGap(card))
            {
                bool missingPerson = !card.AssignedTrustedPersonId.HasValue || card.AssignedTrustedPersonId.Value == Guid.Empty;
                bool missingDoc = string.IsNullOrWhiteSpace(card.DocumentLocationHint) && string.IsNullOrWhiteSpace(card.DigitalStorageLink);

                if (card.Urgency == UrgencyStage.IMMEDIATE) immediateGapsCount++;
                if (missingPerson) delegateGapsCount++;
                if (missingDoc) docGapsCount++;

                var reasons = new List<string>();
                var actions = new List<string>();

                if (missingPerson)
                {
                    reasons.Add("Chưa chỉ định Người Ủy Thác phụ trách");
                    actions.Add("Chỉ định 1 người đáng tin cậy trong mạng lưới để tiếp nhận nhiệm vụ này.");
                }

                if (missingDoc)
                {
                    reasons.Add("Chưa có gợi ý vị trí lưu trữ hồ sơ/giấy tờ liên quan");
                    actions.Add("Ghi chú vị trí vật lý (ngăn tủ, két sắt) hoặc đường dẫn lưu trữ kỹ thuật số.");
                }

                audit.IdentifiedGaps.Add(new PlanGapItemDto
                {
                    CardId = card.Id,
                    CardTitle = card.Title,
                    Stage = card.Urgency,
                    Priority = card.Priority,
                    GapReason = string.Join("; ", reasons),
                    RecommendedAction = string.Join(" ", actions)
                });
            }
        }

        // Actionable Recommendations
        if (immediateGapsCount > 0)
        {
            audit.ActionableRecommendations.Add($"Ưu tiên hàng đầu: Khắc phục ngay {immediateGapsCount} lỗ hổng tiếp quản trong giai đoạn 'Immediate Actions (NOW)' để bảo đảm các việc cấp bách nhất không bị bỏ trống.");
        }

        if (delegateGapsCount > 0)
        {
            audit.ActionableRecommendations.Add($"Có {delegateGapsCount} thẻ hành động chưa được phân công người phụ trách. Hãy mở Ma Trận Phân Quyền hoặc gán trực tiếp để nâng tỷ lệ bao phủ.");
        }

        if (docGapsCount > 0)
        {
            audit.ActionableRecommendations.Add($"Có {docGapsCount} thẻ hành động thiếu thông tin vị trí tài liệu. Việc ghi chú vị trí hồ sơ giúp người tiếp quản xử lý ngay mà không mất thời gian tìm kiếm.");
        }

        if (audit.HasSinglePointOfFailureRisk)
        {
            audit.ActionableRecommendations.Add("Hãy bổ sung thêm Người Ủy Thác hoặc phân bổ bớt các việc thuộc giai đoạn Immediate sang các thành viên khác trong mạng lưới để loại trừ rủi ro SPoF.");
        }

        if (audit.IdentifiedGaps.Count == 0 && cards.Count > 0)
        {
            audit.ActionableRecommendations.Add("Xuất sắc! Kế hoạch tiếp quản của bạn không còn lỗ hổng nào. Hãy định kỳ rà soát mỗi tháng một lần.");
        }

        return audit;
    }
}
