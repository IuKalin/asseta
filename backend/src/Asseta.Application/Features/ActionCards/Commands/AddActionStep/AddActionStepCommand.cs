using Asseta.Application.Features.ActionCards.DTOs;
using MediatR;

namespace Asseta.Application.Features.ActionCards.Commands.AddActionStep;

public record AddActionStepCommand(
    Guid ActionCardId,
    Guid OwnerId,
    string Instruction,
    string? EstimatedDuration = null) : IRequest<ActionCardStepDto>;
