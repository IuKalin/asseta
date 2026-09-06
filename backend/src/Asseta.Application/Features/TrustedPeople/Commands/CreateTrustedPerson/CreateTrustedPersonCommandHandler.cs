using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.TrustedPeople.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.TrustedPeople.Commands.CreateTrustedPerson;

public class CreateTrustedPersonCommandHandler : IRequestHandler<CreateTrustedPersonCommand, TrustedPersonDto>
{
    private readonly IAssetaDbContext _context;
    private readonly IPairingCodeHasher _pairingCodeHasher;

    public CreateTrustedPersonCommandHandler(IAssetaDbContext context, IPairingCodeHasher pairingCodeHasher)
    {
        _context = context;
        _pairingCodeHasher = pairingCodeHasher;
    }

    public async Task<TrustedPersonDto> Handle(CreateTrustedPersonCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra giới hạn 5 Người Ủy Thác tối đa
        var currentCount = await _context.TrustedPeople
            .CountAsync(p => p.OwnerId == request.OwnerId && !p.IsDeleted, cancellationToken);

        if (currentCount >= 5)
        {
            throw new MaxTrustedPeopleExceededException();
        }

        // 2. Chặn trùng lặp SĐT hoặc Email trong cùng tài khoản Owner
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedPhone = request.PhoneNumber.Trim();

        var duplicateExists = await _context.TrustedPeople
            .AnyAsync(p => p.OwnerId == request.OwnerId &&
                           !p.IsDeleted &&
                           (p.Email.ToLower() == normalizedEmail || p.PhoneNumber == normalizedPhone),
                      cancellationToken);

        if (duplicateExists)
        {
            throw new DuplicateContactException();
        }

        // 3. Khởi tạo TrustedPerson
        var person = new TrustedPerson(
            request.FullName.Trim(),
            request.Email.Trim(),
            request.PhoneNumber.Trim(),
            request.Relationship.Trim(),
            request.TrustLevel,
            request.OwnerId,
            request.RoleDescription?.Trim(),
            TrustedPersonStatus.Invited);

        // 4. Sinh mã Pairing Code và tính mã băm HMAC-SHA256
        var plainPairingCode = _pairingCodeHasher.GeneratePairingCode();
        var (codeHash, salt) = _pairingCodeHasher.HashPairingCode(plainPairingCode);
        var expiresAt = DateTime.UtcNow.AddHours(48);

        var pairingCode = new TrustedPersonPairingCode(person.Id, codeHash, salt, expiresAt);
        person.AddPairingCode(pairingCode);

        _context.TrustedPeople.Add(person);
        _context.TrustedPersonPairingCodes.Add(pairingCode);

        // 5. Ghi Audit Log
        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            person.Id,
            "TRUSTED_PERSON_CREATED",
            $"{{\"fullName\":\"{person.FullName}\",\"email\":\"{person.Email}\",\"trustLevel\":{person.TrustLevel}}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return TrustedPersonDto.FromEntity(person, plainPairingCode, expiresAt);
    }
}
