using Asseta.Domain.Entities;
using Asseta.Domain.Enums;

namespace Asseta.Application.Features.TrustedPeople.DTOs;

public record ScopedPermissionDto(
    Guid Id,
    Guid TrustedPersonId,
    string PermissionType,
    Guid? TargetCategoryId,
    string? CategoryNameVi,
    Guid? TargetActionCardId,
    string? ActionCardTitle,
    bool CanView)
{
    public static ScopedPermissionDto FromEntity(TrustedPersonPermission permission)
    {
        return new ScopedPermissionDto(
            permission.Id,
            permission.TrustedPersonId,
            permission.PermissionType.ToString(),
            permission.TargetCategoryId,
            permission.TargetCategory?.NameVi,
            permission.TargetActionCardId,
            permission.TargetActionCard?.Title,
            permission.CanView);
    }
}
