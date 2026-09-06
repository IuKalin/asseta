using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Enums;
using MediatR;

namespace Asseta.Application.Features.ActionCards.Commands.CreateActionCardFromItem;

public record CreateActionCardFromItemCommand(
    Guid OwnerId,
    Guid ContinuityItemId,
    string? TemplateCode = null,
    UrgencyStage? Urgency = null,
    PriorityLevel? Priority = null,
    string? Summary = null) : IRequest<ActionCardDto>;
