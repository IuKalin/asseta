using Asseta.Api.Models;
using Asseta.Application.Features.ContinuityMap.Commands.CreateContinuityItem;
using Asseta.Application.Features.ContinuityMap.Commands.DeleteContinuityItem;
using Asseta.Application.Features.ContinuityMap.Commands.ReorderContinuityItems;
using Asseta.Application.Features.ContinuityMap.Commands.UpdateContinuityItem;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Application.Features.ContinuityMap.Queries.GetContinuityItemById;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Asseta.Api.Controllers;

[Route("api/v1/continuity-items")]
[Route("api/continuity-items")]
public class ContinuityItemsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ContinuityItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContinuityItemDto>>> Create([FromBody] CreateContinuityItemRequest request)
    {
        var command = new CreateContinuityItemCommand(
            CurrentOwnerId,
            request.CategoryId,
            request.Name,
            request.Priority,
            request.DocumentLocationHint,
            request.AssignedTrustedPersonId,
            request.CipherNotesBlob,
            request.CipherNonce,
            request.CipherAuthTag);

        var result = await _mediator.Send(command);
        return CreatedResponse($"/api/v1/continuity-items/{result.Id}", result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContinuityItemDto>>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetContinuityItemByIdQuery(id, CurrentOwnerId));
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContinuityItemDto>>> Update(Guid id, [FromBody] UpdateContinuityItemRequest request)
    {
        var command = new UpdateContinuityItemCommand(
            id,
            CurrentOwnerId,
            request.Name,
            request.Priority,
            request.DocumentLocationHint,
            request.AssignedTrustedPersonId,
            request.CipherNotesBlob,
            request.CipherNonce,
            request.CipherAuthTag,
            request.RowVersion);

        var result = await _mediator.Send(command);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteContinuityItemCommand(id, CurrentOwnerId));
        return OkResponse<object>(new { deleted = result, id });
    }

    [HttpPut("reorder")]
    public async Task<ActionResult<ApiResponse<object>>> Reorder([FromBody] ReorderContinuityItemsRequest request)
    {
        var command = new ReorderContinuityItemsCommand(
            CurrentOwnerId,
            request.CategoryId,
            request.OrderedItemIds);

        var result = await _mediator.Send(command);
        return OkResponse<object>(new { reordered = result });
    }
}

public record CreateContinuityItemRequest(
    Guid CategoryId,
    string Name,
    PriorityLevel Priority = PriorityLevel.IMPORTANT,
    string? DocumentLocationHint = null,
    Guid? AssignedTrustedPersonId = null,
    string? CipherNotesBlob = null,
    string? CipherNonce = null,
    string? CipherAuthTag = null);

public record UpdateContinuityItemRequest(
    string Name,
    PriorityLevel Priority,
    string? DocumentLocationHint,
    Guid? AssignedTrustedPersonId,
    string? CipherNotesBlob,
    string? CipherNonce,
    string? CipherAuthTag,
    int RowVersion);

public record ReorderContinuityItemsRequest(
    Guid CategoryId,
    List<Guid> OrderedItemIds);
