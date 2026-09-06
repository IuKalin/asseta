using MediatR;

namespace Asseta.Application.Features.TrustedPeople.Commands.RevokeTrustedPerson;

public record RevokeTrustedPersonCommand(
    Guid Id,
    Guid OwnerId) : IRequest<bool>;
