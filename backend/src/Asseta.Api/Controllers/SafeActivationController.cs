using Asseta.Api.Models;
using Asseta.Application.Features.SafeActivation.Commands.CancelActivationRequest;
using Asseta.Application.Features.SafeActivation.Commands.ConfirmActivationRequest;
using Asseta.Application.Features.SafeActivation.Commands.DeactivateEmergencyPlan;
using Asseta.Application.Features.SafeActivation.Commands.InitiateActivationRequest;
using Asseta.Application.Features.SafeActivation.Commands.UpdateActivationConfig;
using Asseta.Application.Features.SafeActivation.Commands.VitalityCheckIn;
using Asseta.Application.Features.SafeActivation.DTOs;
using Asseta.Application.Features.SafeActivation.Queries.GetActivationStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Asseta.Api.Controllers;

public record UpdateConfigRequest(int CheckInIntervalDays, int GracePeriodHours, int MinConfirmationsRequired);
public record InitiateActivationApiRequest(Guid TargetOwnerId, string? Reason = null);
public record ConfirmActivationApiRequest(bool IsConfirmed, string? Note = null);

[Route("api/v1/safe-activation")]
[Route("api/safe-activation")]
public class SafeActivationController : BaseApiController
{
    private readonly IMediator _mediator;

    public SafeActivationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lấy trạng thái kích hoạt an toàn, thời hạn check-in, cấu hình và đồng hồ đếm ngược.
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<ApiResponse<ActivationStatusDto>>> GetStatus([FromQuery] Guid? targetOwnerId = null)
    {
        var result = await _mediator.Send(new GetActivationStatusQuery(CurrentOwnerId, targetOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Chủ tài sản thực hiện điểm danh 1-chạm ("Tôi Vẫn Ổn") gia hạn chu kỳ an toàn.
    /// </summary>
    [HttpPost("check-in")]
    public async Task<ActionResult<ApiResponse<ActivationStatusDto>>> VitalityCheckIn()
    {
        var result = await _mediator.Send(new VitalityCheckInCommand(CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Cập nhật cấu hình chu kỳ điểm danh, thời gian đệm và ngưỡng xác nhận.
    /// </summary>
    [HttpPut("config")]
    public async Task<ActionResult<ApiResponse<ActivationConfigDto>>> UpdateConfig([FromBody] UpdateConfigRequest request)
    {
        var result = await _mediator.Send(new UpdateActivationConfigCommand(
            CurrentOwnerId,
            request.CheckInIntervalDays,
            request.GracePeriodHours,
            request.MinConfirmationsRequired));
        return OkResponse(result);
    }

    /// <summary>
    /// Người ủy thác cấp 2/3 (hoặc chủ tài sản) phát động yêu cầu kích hoạt khẩn cấp (bắt đầu thời gian đệm).
    /// </summary>
    [HttpPost("requests")]
    public async Task<ActionResult<ApiResponse<ActivationRequestDto>>> InitiateRequest([FromBody] InitiateActivationApiRequest request)
    {
        var result = await _mediator.Send(new InitiateActivationRequestCommand(
            CurrentOwnerId,
            request.TargetOwnerId,
            request.Reason));
        return CreatedResponse($"/api/v1/safe-activation/requests/{result.Id}", result);
    }

    /// <summary>
    /// Chủ tài sản thực hiện hủy 1-chạm đối với yêu cầu kích hoạt đang trong thời gian đệm.
    /// </summary>
    [HttpPost("requests/{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<bool>>> CancelRequest(Guid id)
    {
        var result = await _mediator.Send(new CancelActivationRequestCommand(CurrentOwnerId, id));
        return OkResponse(result);
    }

    /// <summary>
    /// Người ủy thác cấp 2/3 biểu quyết xác nhận/từ chối yêu cầu kích hoạt khẩn cấp.
    /// </summary>
    [HttpPost("requests/{id:guid}/confirm")]
    public async Task<ActionResult<ApiResponse<ActivationRequestDto>>> ConfirmRequest(Guid id, [FromBody] ConfirmActivationApiRequest request)
    {
        var result = await _mediator.Send(new ConfirmActivationRequestCommand(CurrentOwnerId, id, request.IsConfirmed, request.Note));
        return OkResponse(result);
    }

    /// <summary>
    /// Chủ tài sản khôi phục quyền kiểm soát và tắt trạng thái khẩn cấp (đưa hệ thống về Normal).
    /// </summary>
    [HttpPost("deactivate")]
    public async Task<ActionResult<ApiResponse<bool>>> DeactivatePlan()
    {
        var result = await _mediator.Send(new DeactivateEmergencyPlanCommand(CurrentOwnerId));
        return OkResponse(result);
    }
}
