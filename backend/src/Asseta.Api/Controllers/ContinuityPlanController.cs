using Asseta.Api.Models;
using Asseta.Application.Features.ContinuityPlan.Commands.ToggleActionCardCompletion;
using Asseta.Application.Features.ContinuityPlan.Commands.UpdateActionCardStage;
using Asseta.Application.Features.ContinuityPlan.DTOs;
using Asseta.Application.Features.ContinuityPlan.Queries.GetContinuityPlan;
using Asseta.Application.Features.ContinuityPlan.Queries.GetMyDelegatedPlan;
using Asseta.Application.Features.ContinuityPlan.Queries.GetPlanEmergencyBrief;
using Asseta.Application.Features.ContinuityPlan.Queries.GetPlanReadinessAudit;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Asseta.Api.Controllers;

public record UpdateStageRequest(UrgencyStage NewStage, int RowVersion);
public record ToggleCompletionRequest(int RowVersion);

[Route("api/v1/continuity-plan")]
[Route("api/continuity-plan")]
public class ContinuityPlanController : BaseApiController
{
    private readonly IMediator _mediator;

    public ContinuityPlanController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lấy toàn cảnh Kế Hoạch Tiếp Quản Cá Nhân phân nhóm theo 4 mốc thời gian.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ContinuityPlanDto>>> GetPlan()
    {
        var result = await _mediator.Send(new GetContinuityPlanQuery(CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Trích xuất Bản Tóm Lược Khẩn Cấp Ngoại Tuyến (Zero-Knowledge Offline Emergency Brief).
    /// </summary>
    [HttpGet("emergency-brief")]
    public async Task<ActionResult<ApiResponse<OfflineEmergencyBriefDto>>> GetEmergencyBrief()
    {
        var result = await _mediator.Send(new GetPlanEmergencyBriefQuery(CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Kiểm toán toàn diện mức độ hoàn thiện của Kế hoạch và phân tích rủi ro điểm nghẽn SPoF.
    /// </summary>
    [HttpGet("audit")]
    public async Task<ActionResult<ApiResponse<PlanReadinessAuditDto>>> GetAuditReport()
    {
        var result = await _mediator.Send(new GetPlanReadinessAuditQuery(CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Dành cho Người Ủy Thác truy vấn kế hoạch thuộc phạm vi phân quyền của mình (Zero-Disclosure).
    /// </summary>
    [HttpGet("delegated")]
    public async Task<ActionResult<ApiResponse<ContinuityPlanDto>>> GetMyDelegatedPlan()
    {
        var result = await _mediator.Send(new GetMyDelegatedPlanQuery(CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Điều chỉnh mốc thời gian khẩn cấp của Thẻ hành động (Kéo-thả Timeline Stage).
    /// </summary>
    [HttpPatch("cards/{id:guid}/stage")]
    public async Task<ActionResult<ApiResponse<ContinuityPlanCardItemDto>>> UpdateStage(
        Guid id,
        [FromBody] UpdateStageRequest request)
    {
        var command = new UpdateActionCardStageCommand(id, CurrentOwnerId, request.NewStage, request.RowVersion);
        var result = await _mediator.Send(command);
        return OkResponse(result);
    }

    /// <summary>
    /// Đánh dấu đảo trạng thái đã kiểm tra sẵn sàng thẻ hành động.
    /// </summary>
    [HttpPatch("cards/{id:guid}/toggle-completion")]
    public async Task<ActionResult<ApiResponse<ContinuityPlanCardItemDto>>> ToggleCompletion(
        Guid id,
        [FromBody] ToggleCompletionRequest request)
    {
        var command = new ToggleActionCardCompletionCommand(id, CurrentOwnerId, request.RowVersion);
        var result = await _mediator.Send(command);
        return OkResponse(result);
    }
}
