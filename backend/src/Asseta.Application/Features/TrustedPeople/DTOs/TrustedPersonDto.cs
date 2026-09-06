using Asseta.Domain.Entities;
using Asseta.Domain.Enums;

namespace Asseta.Application.Features.TrustedPeople.DTOs;

public record TrustedPersonDto(
    Guid Id,
    Guid OwnerId,
    Guid? DelegateUserId,
    string FullName,
    string Email,
    string PhoneNumber,
    string Relationship,
    string? RoleDescription,
    int TrustLevel,
    string Status,
    string? ActivePairingCode,
    DateTime? PairingExpiresAt,
    int RowVersion,
    DateTime CreatedAtUtc,
    List<ScopedPermissionDto> Permissions)
{
    public static TrustedPersonDto FromEntity(TrustedPerson person, string? plainPairingCode = null, DateTime? pairingExpiresAt = null)
    {
        var activeCode = person.PairingCodes.FirstOrDefault(c => !c.IsUsed && !c.IsDeleted && c.ExpiresAt > DateTime.UtcNow);
        var permissions = person.Permissions.Select(ScopedPermissionDto.FromEntity).ToList();

        return new TrustedPersonDto(
            person.Id,
            person.OwnerId,
            person.DelegateUserId,
            person.FullName,
            person.Email,
            person.PhoneNumber,
            person.Relationship,
            person.RoleDescription,
            person.TrustLevel,
            person.Status.ToString(),
            plainPairingCode,
            pairingExpiresAt ?? activeCode?.ExpiresAt,
            person.RowVersion,
            person.CreatedAtUtc,
            permissions);
    }
}
