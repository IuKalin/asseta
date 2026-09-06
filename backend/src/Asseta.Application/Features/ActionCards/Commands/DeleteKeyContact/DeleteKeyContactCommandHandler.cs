using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Commands.DeleteKeyContact;

public class DeleteKeyContactCommandHandler : IRequestHandler<DeleteKeyContactCommand, bool>
{
    private readonly IAssetaDbContext _context;

    public DeleteKeyContactCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteKeyContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _context.ActionCardContacts
            .Include(c => c.ActionCard)
            .FirstOrDefaultAsync(c => c.Id == request.ContactId && !c.IsDeleted, cancellationToken);

        if (contact == null)
        {
            throw new NotFoundException(nameof(ActionCardContact), request.ContactId);
        }

        if (contact.ActionCard == null || contact.ActionCard.IsDeleted)
        {
            throw new NotFoundException(nameof(ActionCard), contact.ActionCardId);
        }

        if (contact.ActionCard.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        contact.ActionCard.RemoveContact(contact.Id);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            contact.ActionCardId,
            "ACTION_CONTACT_DELETED",
            $"{{\"contactId\":\"{contact.Id}\"}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
