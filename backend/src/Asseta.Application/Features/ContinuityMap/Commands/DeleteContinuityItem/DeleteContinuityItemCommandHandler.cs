using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityMap.Commands.DeleteContinuityItem;

public class DeleteContinuityItemCommandHandler : IRequestHandler<DeleteContinuityItemCommand, bool>
{
    private readonly IAssetaDbContext _context;

    public DeleteContinuityItemCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteContinuityItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ContinuityItems
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException(nameof(ContinuityItem), request.Id);
        }

        if (item.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        item.SoftDelete();

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            item.Id,
            "SOFT_DELETED",
            $"{{\"deletedAt\":\"{DateTime.UtcNow:O}\"}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
