using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.TrustedPeople.Commands.RevokeTrustedPerson;

public class RevokeTrustedPersonCommandHandler : IRequestHandler<RevokeTrustedPersonCommand, bool>
{
    private readonly IAssetaDbContext _context;

    public RevokeTrustedPersonCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RevokeTrustedPersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.TrustedPeople
            .Include(p => p.Permissions)
            .Include(p => p.PairingCodes)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (person == null)
        {
            throw new NotFoundException(nameof(TrustedPerson), request.Id);
        }

        if (person.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        // 1. Thu hồi quyền và chuyển trạng thái
        person.Revoke();
        person.MarkDeleted();

        // 2. Vô hiệu hóa mã ghép đôi
        foreach (var code in person.PairingCodes.Where(c => !c.IsUsed && !c.IsDeleted))
        {
            code.Invalidate();
        }

        // 3. Xóa quyền trong ma trận phân quyền
        var permissions = await _context.TrustedPersonPermissions
            .Where(p => p.TrustedPersonId == person.Id)
            .ToListAsync(cancellationToken);
        _context.TrustedPersonPermissions.RemoveRange(permissions);

        // 4. [REQ-TRUST-010]: Unassign khỏi các ContinuityItems
        var assignedItems = await _context.ContinuityItems
            .Where(i => i.OwnerId == request.OwnerId && i.AssignedTrustedPersonId == person.Id && !i.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var item in assignedItems)
        {
            item.UnassignTrustedPerson();
        }

        // 5. Unassign khỏi các ActionCards
        var assignedCards = await _context.ActionCards
            .Where(c => c.OwnerId == request.OwnerId && c.AssignedTrustedPersonId == person.Id && !c.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var card in assignedCards)
        {
            card.UnassignTrustedPerson();
        }

        // 6. Ghi Audit Log
        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            person.Id,
            "TRUSTED_PERSON_REVOKED",
            $"{{\"unassignedItemsCount\":{assignedItems.Count},\"unassignedCardsCount\":{assignedCards.Count}}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
