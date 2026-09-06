using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.TrustedPeople.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.TrustedPeople.Queries.GetTrustedPeople;

public class GetTrustedPeopleQueryHandler : IRequestHandler<GetTrustedPeopleQuery, List<TrustedPersonDto>>
{
    private readonly IAssetaDbContext _context;

    public GetTrustedPeopleQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<List<TrustedPersonDto>> Handle(GetTrustedPeopleQuery request, CancellationToken cancellationToken)
    {
        var people = await _context.TrustedPeople
            .Include(p => p.PairingCodes)
            .Include(p => p.Permissions)
                .ThenInclude(perm => perm.TargetCategory)
            .Include(p => p.Permissions)
                .ThenInclude(perm => perm.TargetActionCard)
            .Where(p => p.OwnerId == request.OwnerId && !p.IsDeleted)
            .OrderBy(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return people.Select(p => TrustedPersonDto.FromEntity(p)).ToList();
    }
}
