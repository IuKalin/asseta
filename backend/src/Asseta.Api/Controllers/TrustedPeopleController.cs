using Asseta.Api.Models;
using Asseta.Application.Features.TrustedPeople.Commands.ClaimPairingCode;
using Asseta.Application.Features.TrustedPeople.Commands.CreateTrustedPerson;
using Asseta.Application.Features.TrustedPeople.Commands.RegeneratePairingCode;
using Asseta.Application.Features.TrustedPeople.Commands.RevokeTrustedPerson;
using Asseta.Application.Features.TrustedPeople.Commands.UpdateScopedPermissions;
using Asseta.Application.Features.TrustedPeople.Commands.UpdateTrustedPerson;
using Asseta.Application.Features.TrustedPeople.DTOs;
using Asseta.Application.Features.TrustedPeople.Queries.GetMyDelegatedRoles;
using Asseta.Application.Features.TrustedPeople.Queries.GetTrustedPeople;
using Asseta.Application.Features.TrustedPeople.Queries.GetTrustedPersonDetail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Asseta.Api.Controllers;

[Route("api/v1/trusted-people")]
[Route("api/trusted-people")]
public class TrustedPeopleController : BaseApiController
{
    private readonly IMediator _mediator;

    public TrustedPeopleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lấy danh sách Người Ủy Thác của chủ tài sản hiện tại.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TrustedPersonDto>>>> GetAll()
    {
        var result = await _mediator.Send(new GetTrustedPeopleQuery(CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Lấy chi tiết thông tin một Người Ủy Thác theo ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TrustedPersonDto>>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetTrustedPersonDetailQuery(id, CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Tạo mới Người Ủy Thác và tự động sinh mã ghép đôi 6 ký tự.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TrustedPersonDto>>> Create([FromBody] CreateTrustedPersonRequest request)
    {
        var command = new CreateTrustedPersonCommand(
            CurrentOwnerId,
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Relationship,
            request.TrustLevel,
            request.RoleDescription);

        var result = await _mediator.Send(command);
        return CreatedResponse($"/api/trusted-people/{result.Id}", result);
    }

    /// <summary>
    /// Cập nhật hồ sơ thông tin và cấp bậc tin cậy của Người Ủy Thác.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TrustedPersonDto>>> Update(Guid id, [FromBody] UpdateTrustedPersonRequest request)
    {
        var command = new UpdateTrustedPersonCommand(
            id,
            CurrentOwnerId,
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Relationship,
            request.TrustLevel,
            request.ExpectedRowVersion,
            request.RoleDescription);

        var result = await _mediator.Send(command);
        return OkResponse(result);
    }

    /// <summary>
    /// Thu hồi quyền hoặc xóa mềm Người Ủy Thác.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Revoke(Guid id)
    {
        var result = await _mediator.Send(new RevokeTrustedPersonCommand(id, CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Cấp lại mã ghép đôi mới khi mã cũ hết hạn (48h).
    /// </summary>
    [HttpPost("{id:guid}/pairing-code/regenerate")]
    public async Task<ActionResult<ApiResponse<TrustedPersonDto>>> RegeneratePairingCode(Guid id)
    {
        var result = await _mediator.Send(new RegeneratePairingCodeCommand(id, CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Người Ủy Thác nhập mã Pairing Code để liên kết danh tính trên thiết bị di động/web.
    /// </summary>
    [HttpPost("pairing/claim")]
    public async Task<ActionResult<ApiResponse<ClaimPairingResultDto>>> ClaimPairingCode([FromBody] ClaimPairingCodeRequest request)
    {
        var result = await _mediator.Send(new ClaimPairingCodeCommand(CurrentOwnerId, request.PairingCode));
        return OkResponse(result);
    }

    /// <summary>
    /// Cập nhật Ma Trận Phân Quyền chi tiết theo Danh mục hoặc Thẻ hành động.
    /// </summary>
    [HttpPut("{id:guid}/permissions")]
    public async Task<ActionResult<ApiResponse<List<ScopedPermissionDto>>>> UpdatePermissions(
        Guid id,
        [FromBody] UpdateScopedPermissionsRequest request)
    {
        var command = new UpdateScopedPermissionsCommand(
            id,
            CurrentOwnerId,
            request.CategoryPermissions,
            request.ActionCardPermissions);

        var result = await _mediator.Send(command);
        return OkResponse(result);
    }

    /// <summary>
    /// Xem danh sách các vai trò được ủy thác dành cho Delegate (Zero-Disclosure).
    /// </summary>
    [HttpGet("my-delegated-roles")]
    public async Task<ActionResult<ApiResponse<List<DelegatedRoleDto>>>> GetMyDelegatedRoles()
    {
        var result = await _mediator.Send(new GetMyDelegatedRolesQuery(CurrentOwnerId));
        return OkResponse(result);
    }
}

public record CreateTrustedPersonRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    string Relationship,
    int TrustLevel,
    string? RoleDescription = null);

public record UpdateTrustedPersonRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    string Relationship,
    int TrustLevel,
    int ExpectedRowVersion,
    string? RoleDescription = null);

public record ClaimPairingCodeRequest(string PairingCode);

public record UpdateScopedPermissionsRequest(
    List<CategoryPermissionInput>? CategoryPermissions = null,
    List<ActionCardPermissionInput>? ActionCardPermissions = null);
