using Asseta.Application.Features.TrustedPeople.DTOs;
using MediatR;

namespace Asseta.Application.Features.TrustedPeople.Commands.UpdateTrustedPerson;

public record UpdateTrustedPersonCommand(
    Guid Id,
    Guid OwnerId,
    string FullName,
    string Email,
    string PhoneNumber,
    string Relationship,
    int TrustLevel,
    int ExpectedRowVersion,
    string? RoleDescription = null) : IRequest<TrustedPersonDto>;
