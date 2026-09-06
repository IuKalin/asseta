using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.TrustedPeople.Queries.GetMyDelegatedRoles;

public class GetMyDelegatedRolesQueryHandler : IRequestHandler<GetMyDelegatedRolesQuery, List<DelegatedRoleDto>>
{
    private readonly IAssetaDbContext _context;

    public GetMyDelegatedRolesQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<List<DelegatedRoleDto>> Handle(GetMyDelegatedRolesQuery request, CancellationToken cancellationToken)
    {
        var delegatedPeople = await _context.TrustedPeople
            .Where(p => p.DelegateUserId == request.DelegateUserId && !p.IsDeleted && p.Status == TrustedPersonStatus.Active)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        if (!delegatedPeople.Any())
        {
            return new List<DelegatedRoleDto>();
        }

        var ownerIds = delegatedPeople.Select(p => p.OwnerId).Distinct().ToList();
        var owners = await _context.Users
            .Where(u => ownerIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

        return delegatedPeople.Select(p => new DelegatedRoleDto(
            p.Id,
            p.OwnerId,
            owners.TryGetValue(p.OwnerId, out var name) ? name : "Chủ tài sản",
            p.Relationship,
            p.RoleDescription,
            p.TrustLevel,
            p.Status.ToString(),
            p.CreatedAtUtc)).ToList();
    }
}
