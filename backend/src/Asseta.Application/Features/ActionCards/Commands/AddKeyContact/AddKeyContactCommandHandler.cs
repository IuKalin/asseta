using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Commands.AddKeyContact;

public class AddKeyContactCommandHandler : IRequestHandler<AddKeyContactCommand, ActionCardContactDto>
{
    private readonly IAssetaDbContext _context;

    public AddKeyContactCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActionCardContactDto> Handle(AddKeyContactCommand request, CancellationToken cancellationToken)
    {
        var card = await _context.ActionCards
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == request.ActionCardId && !c.IsDeleted, cancellationToken);

        if (card == null)
        {
            throw new NotFoundException(nameof(ActionCard), request.ActionCardId);
        }

        if (card.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        var contact = card.AddContact(
            request.ContactName,
            request.RelationshipOrRole,
            request.PhoneNumber,
            request.Email,
            request.ContactNotes);

        _context.ActionCardContacts.Add(contact);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            card.Id,
            "ACTION_CONTACT_ADDED",
            $"{{\"contactId\":\"{contact.Id}\",\"contactName\":\"{contact.ContactName}\"}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return ActionCardContactDto.FromEntity(contact);
    }
}
