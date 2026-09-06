using Asseta.Application.Features.ActionCards.DTOs;
using MediatR;

namespace Asseta.Application.Features.ActionCards.Commands.ReorderActionSteps;

public record ReorderActionStepsCommand(
    Guid ActionCardId,
    Guid OwnerId,
    IReadOnlyList<Guid> OrderedStepIds) : IRequest<IReadOnlyList<ActionCardStepDto>>;
