using Asseta.Application.Features.TrustedPeople.DTOs;
using MediatR;

namespace Asseta.Application.Features.TrustedPeople.Commands.CreateTrustedPerson;

public record CreateTrustedPersonCommand(
    Guid OwnerId,
    string FullName,
    string Email,
    string PhoneNumber,
    string Relationship,
    int TrustLevel,
    string? RoleDescription = null) : IRequest<TrustedPersonDto>;
