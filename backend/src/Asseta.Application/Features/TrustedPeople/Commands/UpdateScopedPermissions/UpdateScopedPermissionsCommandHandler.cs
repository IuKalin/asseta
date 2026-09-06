using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.TrustedPeople.DTOs;
using Asseta.Domain.Constants;
using Asseta.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.TrustedPeople.Commands.UpdateScopedPermissions;

public class UpdateScopedPermissionsCommandHandler : IRequestHandler<UpdateScopedPermissionsCommand, List<ScopedPermissionDto>>
{
    private readonly IAssetaDbContext _context;

    public UpdateScopedPermissionsCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<List<ScopedPermissionDto>> Handle(UpdateScopedPermissionsCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.TrustedPeople
            .Include(p => p.Permissions)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (person == null)
        {
            throw new NotFoundException(nameof(TrustedPerson), request.Id);
        }

        if (person.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        if (person.TrustLevel == 1)
        {
            throw new ValidationException("Người ủy thác ở cấp bậc Notice Only (Level 1) không thể phân quyền xem nội dung chi tiết.");
        }

        var existingPermissions = await _context.TrustedPersonPermissions
            .Where(p => p.TrustedPersonId == person.Id)
            .ToListAsync(cancellationToken);

        _context.TrustedPersonPermissions.RemoveRange(existingPermissions);

        var newPermissions = new List<TrustedPersonPermission>();

        if (request.CategoryPermissions != null && request.CategoryPermissions.Any())
        {
            var categoryIds = request.CategoryPermissions
                .Select(c => CategoryCodes.ResolveCanonicalId(c.CategoryId))
                .Distinct()
                .ToList();

            var validCategories = await _context.ContinuityCategories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            foreach (var cp in request.CategoryPermissions)
            {
                var canonicalId = CategoryCodes.ResolveCanonicalId(cp.CategoryId);
                if (validCategories.Any(vc => vc.Id == canonicalId))
                {
                    var perm = TrustedPersonPermission.CreateForCategory(person.Id, canonicalId, cp.CanView);
                    newPermissions.Add(perm);
                    _context.TrustedPersonPermissions.Add(perm);
                }
            }
        }

        if (request.ActionCardPermissions != null && request.ActionCardPermissions.Any())
        {
            var cardIds = request.ActionCardPermissions.Select(a => a.ActionCardId).Distinct().ToList();
            var validCards = await _context.ActionCards
                .Where(c => cardIds.Contains(c.Id) && c.OwnerId == request.OwnerId && !c.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var ap in request.ActionCardPermissions)
            {
                if (validCards.Any(vc => vc.Id == ap.ActionCardId))
                {
                    var perm = TrustedPersonPermission.CreateForActionCard(person.Id, ap.ActionCardId, ap.CanView);
                    newPermissions.Add(perm);
                    _context.TrustedPersonPermissions.Add(perm);
                }
            }
        }

        person.SetPermissions(newPermissions);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            person.Id,
            "TRUSTED_PERSON_PERMISSIONS_UPDATED",
            $"{{\"categoriesCount\":{request.CategoryPermissions?.Count ?? 0},\"cardsCount\":{request.ActionCardPermissions?.Count ?? 0}}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        // Load navigation properties để trả về đầy đủ DTO
        var savedPermissions = await _context.TrustedPersonPermissions
            .Include(p => p.TargetCategory)
            .Include(p => p.TargetActionCard)
            .Where(p => p.TrustedPersonId == person.Id && !p.IsDeleted)
            .ToListAsync(cancellationToken);

        return savedPermissions.Select(ScopedPermissionDto.FromEntity).ToList();
    }
}
