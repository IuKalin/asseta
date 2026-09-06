using MediatR;

namespace Asseta.Application.Features.ActionCards.Commands.DeleteActionCard;

public record DeleteActionCardCommand(Guid Id, Guid OwnerId) : IRequest<bool>;
