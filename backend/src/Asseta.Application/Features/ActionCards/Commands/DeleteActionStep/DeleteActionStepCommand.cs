using MediatR;

namespace Asseta.Application.Features.ActionCards.Commands.DeleteActionStep;

public record DeleteActionStepCommand(Guid StepId, Guid OwnerId) : IRequest<bool>;
