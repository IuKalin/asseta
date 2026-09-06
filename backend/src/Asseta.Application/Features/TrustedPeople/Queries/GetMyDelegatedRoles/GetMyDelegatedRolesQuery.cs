using MediatR;

namespace Asseta.Application.Features.TrustedPeople.Queries.GetMyDelegatedRoles;

public record DelegatedRoleDto(
    Guid TrustedPersonId,
    Guid OwnerId,
    string OwnerDisplayName,
    string Relationship,
    string? RoleDescription,
    int TrustLevel,
    string Status,
    DateTime CreatedAtUtc);

public record GetMyDelegatedRolesQuery(Guid DelegateUserId) : IRequest<List<DelegatedRoleDto>>;
