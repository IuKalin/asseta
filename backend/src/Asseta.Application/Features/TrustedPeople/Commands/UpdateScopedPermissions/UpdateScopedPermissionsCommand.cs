using Asseta.Application.Features.TrustedPeople.DTOs;
using MediatR;

namespace Asseta.Application.Features.TrustedPeople.Commands.UpdateScopedPermissions;

public record CategoryPermissionInput(Guid CategoryId, bool CanView = true);
public record ActionCardPermissionInput(Guid ActionCardId, bool CanView = true);

public record UpdateScopedPermissionsCommand(
    Guid Id,
    Guid OwnerId,
    List<CategoryPermissionInput>? CategoryPermissions = null,
    List<ActionCardPermissionInput>? ActionCardPermissions = null) : IRequest<List<ScopedPermissionDto>>;
