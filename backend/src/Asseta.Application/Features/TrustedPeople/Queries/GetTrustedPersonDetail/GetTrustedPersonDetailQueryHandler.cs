using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.TrustedPeople.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.TrustedPeople.Queries.GetTrustedPersonDetail;

public class GetTrustedPersonDetailQueryHandler : IRequestHandler<GetTrustedPersonDetailQuery, TrustedPersonDto>
{
    private readonly IAssetaDbContext _context;

    public GetTrustedPersonDetailQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<TrustedPersonDto> Handle(GetTrustedPersonDetailQuery request, CancellationToken cancellationToken)
    {
        var person = await _context.TrustedPeople
            .Include(p => p.PairingCodes)
            .Include(p => p.Permissions)
                .ThenInclude(perm => perm.TargetCategory)
            .Include(p => p.Permissions)
                .ThenInclude(perm => perm.TargetActionCard)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (person == null)
        {
            throw new NotFoundException(nameof(TrustedPerson), request.Id);
        }

        if (person.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        return TrustedPersonDto.FromEntity(person);
    }
}
