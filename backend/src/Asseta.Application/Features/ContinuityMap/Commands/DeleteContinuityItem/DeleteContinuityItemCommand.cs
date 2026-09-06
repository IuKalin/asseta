using MediatR;

namespace Asseta.Application.Features.ContinuityMap.Commands.DeleteContinuityItem;

public record DeleteContinuityItemCommand(Guid Id, Guid OwnerId) : IRequest<bool>;
