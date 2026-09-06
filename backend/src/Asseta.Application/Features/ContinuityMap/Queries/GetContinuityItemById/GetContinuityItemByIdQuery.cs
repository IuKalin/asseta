using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityMap.Queries.GetContinuityItemById;

public record GetContinuityItemByIdQuery(Guid Id, Guid OwnerId) : IRequest<ContinuityItemDto>;

public class GetContinuityItemByIdQueryHandler : IRequestHandler<GetContinuityItemByIdQuery, ContinuityItemDto>
{
    private readonly IAssetaDbContext _context;

    public GetContinuityItemByIdQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ContinuityItemDto> Handle(GetContinuityItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.ContinuityItems
            .Include(i => i.Category)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException(nameof(ContinuityItem), request.Id);
        }

        if (item.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        return ContinuityItemDto.FromEntity(item);
    }
}
