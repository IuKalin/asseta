using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.TrustedPeople.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.TrustedPeople.Commands.RegeneratePairingCode;

public class RegeneratePairingCodeCommandHandler : IRequestHandler<RegeneratePairingCodeCommand, TrustedPersonDto>
{
    private readonly IAssetaDbContext _context;
    private readonly IPairingCodeHasher _pairingCodeHasher;

    public RegeneratePairingCodeCommandHandler(IAssetaDbContext context, IPairingCodeHasher pairingCodeHasher)
    {
        _context = context;
        _pairingCodeHasher = pairingCodeHasher;
    }

    public async Task<TrustedPersonDto> Handle(RegeneratePairingCodeCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.TrustedPeople
            .Include(p => p.PairingCodes)
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

        var plainPairingCode = _pairingCodeHasher.GeneratePairingCode();
        var (codeHash, salt) = _pairingCodeHasher.HashPairingCode(plainPairingCode);
        var expiresAt = DateTime.UtcNow.AddHours(48);

        var newCode = new TrustedPersonPairingCode(person.Id, codeHash, salt, expiresAt);
        person.AddPairingCode(newCode);

        _context.TrustedPersonPairingCodes.Add(newCode);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            person.Id,
            "TRUSTED_PERSON_PAIRING_CODE_REGENERATED",
            $"{{\"expiresAt\":\"{expiresAt:O}\"}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return TrustedPersonDto.FromEntity(person, plainPairingCode, expiresAt);
    }
}
