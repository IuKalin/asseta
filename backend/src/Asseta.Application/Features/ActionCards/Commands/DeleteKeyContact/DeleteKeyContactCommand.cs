using MediatR;

namespace Asseta.Application.Features.ActionCards.Commands.DeleteKeyContact;

public record DeleteKeyContactCommand(Guid ContactId, Guid OwnerId) : IRequest<bool>;
