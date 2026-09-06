using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Domain.Constants;
using Asseta.Domain.Entities;
using Asseta.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityMap.Commands.CreateContinuityItem;

public class CreateContinuityItemCommandHandler : IRequestHandler<CreateContinuityItemCommand, ContinuityItemDto>
{
    private readonly IAssetaDbContext _context;

    public CreateContinuityItemCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ContinuityItemDto> Handle(CreateContinuityItemCommand request, CancellationToken cancellationToken)
    {
        var targetCategoryId = CategoryCodes.ResolveCanonicalId(request.CategoryId);
        var category = await _context.ContinuityCategories
            .FirstOrDefaultAsync(c => c.Id == targetCategoryId, cancellationToken);

        if (category == null)
        {
            category = await _context.ContinuityCategories.FirstOrDefaultAsync(cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(ContinuityCategory), request.CategoryId);
            }
        }

        CipherBlobPayload? encryptedNotes = null;
        if (!string.IsNullOrWhiteSpace(request.CipherNotesBlob) &&
            !string.IsNullOrWhiteSpace(request.CipherNonce) &&
            !string.IsNullOrWhiteSpace(request.CipherAuthTag))
        {
            encryptedNotes = new CipherBlobPayload(
                request.CipherNotesBlob,
                request.CipherNonce,
                request.CipherAuthTag);
        }

        var item = new ContinuityItem(
            request.OwnerId,
            category.Id,
            request.Name,
            request.Priority,
            request.DocumentLocationHint,
            request.AssignedTrustedPersonId,
            encryptedNotes);

        _context.ContinuityItems.Add(item);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            item.Id,
            "CREATED",
            $"{{\"name\":\"{item.Name}\",\"priority\":\"{item.Priority}\"}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return ContinuityItemDto.FromEntity(item, category.Code);
    }
}
