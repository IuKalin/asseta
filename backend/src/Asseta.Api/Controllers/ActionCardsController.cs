using Asseta.Api.Models;
using Asseta.Application.Features.ActionCards.Commands.AddActionStep;
using Asseta.Application.Features.ActionCards.Commands.AddKeyContact;
using Asseta.Application.Features.ActionCards.Commands.CreateActionCard;
using Asseta.Application.Features.ActionCards.Commands.CreateActionCardFromItem;
using Asseta.Application.Features.ActionCards.Commands.DeleteActionCard;
using Asseta.Application.Features.ActionCards.Commands.DeleteActionStep;
using Asseta.Application.Features.ActionCards.Commands.DeleteKeyContact;
using Asseta.Application.Features.ActionCards.Commands.ReorderActionSteps;
using Asseta.Application.Features.ActionCards.Commands.UpdateActionCard;
using Asseta.Application.Features.ActionCards.Commands.UpdateActionStep;
using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Application.Features.ActionCards.Queries.GetActionCardById;
using Asseta.Application.Features.ActionCards.Queries.GetActionCards;
using Asseta.Application.Features.ActionCards.Queries.GetActionCardTemplates;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Asseta.Api.Controllers;

[Route("api/v1/action-cards")]
[Route("api/action-cards")]
public class ActionCardsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ActionCardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all action cards for current user with optional urgency, category and text filters.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ActionCardDto>>>> GetAll(
        [FromQuery] UrgencyStage? urgency,
        [FromQuery] Guid? categoryId,
        [FromQuery] string? search)
    {
        var result = await _mediator.Send(new GetActionCardsQuery(CurrentOwnerId, urgency, categoryId, search));
        return OkResponse(result);
    }

    /// <summary>
    /// Retrieves pre-configured action card templates.
    /// </summary>
    [HttpGet("templates")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ActionCardTemplateDto>>>> GetTemplates(
        [FromQuery] string? categoryCode)
    {
        var result = await _mediator.Send(new GetActionCardTemplatesQuery(categoryCode));
        return OkResponse(result);
    }

    /// <summary>
    /// Retrieves a single action card by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ActionCardDto>>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetActionCardByIdQuery(id, CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Creates a new action card.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ActionCardDto>>> Create([FromBody] CreateActionCardRequest request)
    {
        var command = new CreateActionCardCommand(
            CurrentOwnerId,
            request.CategoryId,
            request.Title,
            request.Urgency,
            request.Priority,
            request.Summary,
            request.AssignedTrustedPersonId,
            request.DocumentLocationHint,
            request.DigitalStorageLink,
            request.CipherInstructionsBlob,
            request.CipherNonce,
            request.CipherAuthTag);

        var result = await _mediator.Send(command);
        return CreatedResponse($"/api/v1/action-cards/{result.Id}", result);
    }

    /// <summary>
    /// Generates an action card linked to an existing Continuity Item, optionally using a template.
    /// </summary>
    [HttpPost("from-item")]
    public async Task<ActionResult<ApiResponse<ActionCardDto>>> CreateFromItem([FromBody] CreateFromItemRequest request)
    {
        var command = new CreateActionCardFromItemCommand(
            CurrentOwnerId,
            request.ContinuityItemId,
            request.TemplateCode,
            request.Urgency,
            request.Priority,
            request.Summary);

        var result = await _mediator.Send(command);
        return CreatedResponse($"/api/v1/action-cards/{result.Id}", result);
    }

    /// <summary>
    /// Updates an existing action card with optimistic locking.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ActionCardDto>>> Update(Guid id, [FromBody] UpdateActionCardRequest request)
    {
        var command = new UpdateActionCardCommand(
            id,
            CurrentOwnerId,
            request.Title,
            request.Urgency,
            request.Priority,
            request.RowVersion,
            request.Summary,
            request.AssignedTrustedPersonId,
            request.DocumentLocationHint,
            request.DigitalStorageLink,
            request.CipherInstructionsBlob,
            request.CipherNonce,
            request.CipherAuthTag);

        var result = await _mediator.Send(command);
        return OkResponse(result);
    }

    /// <summary>
    /// Soft deletes an action card.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteActionCardCommand(id, CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Adds a step to an action card (up to 20 steps).
    /// </summary>
    [HttpPost("{id:guid}/steps")]
    public async Task<ActionResult<ApiResponse<ActionCardStepDto>>> AddStep(Guid id, [FromBody] AddStepRequest request)
    {
        var command = new AddActionStepCommand(id, CurrentOwnerId, request.Instruction, request.EstimatedDuration);
        var result = await _mediator.Send(command);
        return CreatedResponse($"/api/v1/action-cards/{id}/steps/{result.Id}", result);
    }

    /// <summary>
    /// Updates a specific step on an action card.
    /// </summary>
    [HttpPut("{cardId:guid}/steps/{stepId:guid}")]
    public async Task<ActionResult<ApiResponse<ActionCardStepDto>>> UpdateStep(
        Guid cardId,
        Guid stepId,
        [FromBody] UpdateStepRequest request)
    {
        var command = new UpdateActionStepCommand(stepId, CurrentOwnerId, request.Instruction, request.EstimatedDuration, request.IsCompleted);
        var result = await _mediator.Send(command);
        return OkResponse(result);
    }

    /// <summary>
    /// Removes a step from an action card.
    /// </summary>
    [HttpDelete("{cardId:guid}/steps/{stepId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteStep(Guid cardId, Guid stepId)
    {
        var result = await _mediator.Send(new DeleteActionStepCommand(stepId, CurrentOwnerId));
        return OkResponse(result);
    }

    /// <summary>
    /// Reorders the steps on an action card.
    /// </summary>
    [HttpPost("{id:guid}/steps/reorder")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ActionCardStepDto>>>> ReorderSteps(
        Guid id,
        [FromBody] ReorderStepsRequest request)
    {
        var command = new ReorderActionStepsCommand(id, CurrentOwnerId, request.OrderedStepIds);
        var result = await _mediator.Send(command);
        return OkResponse(result);
    }

    /// <summary>
    /// Adds a key contact to an action card (up to 5 contacts).
    /// </summary>
    [HttpPost("{id:guid}/contacts")]
    public async Task<ActionResult<ApiResponse<ActionCardContactDto>>> AddContact(
        Guid id,
        [FromBody] AddContactRequest request)
    {
        var command = new AddKeyContactCommand(
            id,
            CurrentOwnerId,
            request.ContactName,
            request.RelationshipOrRole,
            request.PhoneNumber,
            request.Email,
            request.ContactNotes);

        var result = await _mediator.Send(command);
        return CreatedResponse($"/api/v1/action-cards/{id}/contacts/{result.Id}", result);
    }

    /// <summary>
    /// Removes a key contact from an action card.
    /// </summary>
    [HttpDelete("{cardId:guid}/contacts/{contactId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteContact(Guid cardId, Guid contactId)
    {
        var result = await _mediator.Send(new DeleteKeyContactCommand(contactId, CurrentOwnerId));
        return OkResponse(result);
    }
}

// Request Models
public record CreateActionCardRequest(
    Guid CategoryId,
    string Title,
    UrgencyStage Urgency,
    PriorityLevel Priority,
    string? Summary = null,
    Guid? AssignedTrustedPersonId = null,
    string? DocumentLocationHint = null,
    string? DigitalStorageLink = null,
    string? CipherInstructionsBlob = null,
    string? CipherNonce = null,
    string? CipherAuthTag = null);

public record CreateFromItemRequest(
    Guid ContinuityItemId,
    string? TemplateCode = null,
    UrgencyStage? Urgency = null,
    PriorityLevel? Priority = null,
    string? Summary = null);

public record UpdateActionCardRequest(
    string Title,
    UrgencyStage Urgency,
    PriorityLevel Priority,
    int RowVersion,
    string? Summary = null,
    Guid? AssignedTrustedPersonId = null,
    string? DocumentLocationHint = null,
    string? DigitalStorageLink = null,
    string? CipherInstructionsBlob = null,
    string? CipherNonce = null,
    string? CipherAuthTag = null);

public record AddStepRequest(string Instruction, string? EstimatedDuration = null);

public record UpdateStepRequest(string Instruction, string? EstimatedDuration = null, bool? IsCompleted = null);

public record ReorderStepsRequest(IReadOnlyList<Guid> OrderedStepIds);

public record AddContactRequest(
    string ContactName,
    string RelationshipOrRole,
    string? PhoneNumber = null,
    string? Email = null,
    string? ContactNotes = null);
