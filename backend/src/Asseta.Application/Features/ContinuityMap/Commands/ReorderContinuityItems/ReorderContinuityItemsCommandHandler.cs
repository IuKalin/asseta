using Asseta.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityMap.Commands.ReorderContinuityItems;

public class ReorderContinuityItemsCommandHandler : IRequestHandler<ReorderContinuityItemsCommand, bool>
{
    private readonly IAssetaDbContext _context;

    public ReorderContinuityItemsCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ReorderContinuityItemsCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.ContinuityItems
            .Where(i => i.OwnerId == request.OwnerId && i.CategoryId == request.CategoryId)
            .ToListAsync(cancellationToken);

        for (int order = 0; order < request.OrderedItemIds.Count; order++)
        {
            var targetId = request.OrderedItemIds[order];
            var item = items.FirstOrDefault(i => i.Id == targetId);
            if (item != null)
            {
                item.UpdateSortOrder(order + 1);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
