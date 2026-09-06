using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityMap.Commands.UpdateContinuityItem;

public class UpdateContinuityItemCommandHandler : IRequestHandler<UpdateContinuityItemCommand, ContinuityItemDto>
{
    private readonly IAssetaDbContext _context;

    public UpdateContinuityItemCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ContinuityItemDto> Handle(UpdateContinuityItemCommand request, CancellationToken cancellationToken)
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

        if (item.RowVersion != request.RowVersion)
        {
            throw new ConcurrencyException($"Item has been modified by another process. Expected version {request.RowVersion}, but found {item.RowVersion}.");
        }

        item.UpdateDetails(
            request.Name,
            request.Priority,
            request.DocumentLocationHint,
            request.AssignedTrustedPersonId);

        if (!string.IsNullOrWhiteSpace(request.CipherNotesBlob) &&
            !string.IsNullOrWhiteSpace(request.CipherNonce) &&
            !string.IsNullOrWhiteSpace(request.CipherAuthTag))
        {
            item.SetEncryptedNotes(new CipherBlobPayload(
                request.CipherNotesBlob,
                request.CipherNonce,
                request.CipherAuthTag));
        }

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            item.Id,
            "UPDATED",
            $"{{\"name\":\"{item.Name}\",\"priority\":\"{item.Priority}\",\"newVersion\":{item.RowVersion}}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return ContinuityItemDto.FromEntity(item);
    }
}
