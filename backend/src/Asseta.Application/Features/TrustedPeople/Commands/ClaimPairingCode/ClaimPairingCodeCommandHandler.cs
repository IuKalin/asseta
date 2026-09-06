using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.TrustedPeople.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.TrustedPeople.Commands.ClaimPairingCode;

public class ClaimPairingCodeCommandHandler : IRequestHandler<ClaimPairingCodeCommand, ClaimPairingResultDto>
{
    private readonly IAssetaDbContext _context;
    private readonly IPairingCodeHasher _pairingCodeHasher;

    public ClaimPairingCodeCommandHandler(IAssetaDbContext context, IPairingCodeHasher pairingCodeHasher)
    {
        _context = context;
        _pairingCodeHasher = pairingCodeHasher;
    }

    public async Task<ClaimPairingResultDto> Handle(ClaimPairingCodeCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var normalizedCode = request.PairingCode.Trim().ToUpperInvariant();

        // Tìm các pairing code đang hoạt động
        var candidateCodes = await _context.TrustedPersonPairingCodes
            .Include(c => c.TrustedPerson)
            .Where(c => !c.IsUsed && !c.IsDeleted && c.ExpiresAt > now)
            .ToListAsync(cancellationToken);

        TrustedPersonPairingCode? matchedCode = null;

        foreach (var candidate in candidateCodes)
        {
            if (candidate.IsLockedOut(now))
            {
                // Nếu mã này đang bị khóa, kiểm tra xem có đúng là mã đang nhập không
                if (_pairingCodeHasher.VerifyPairingCode(normalizedCode, candidate.CodeHash, candidate.Salt))
                {
                    throw new PairingAttemptsExceededException();
                }
            }
            else if (_pairingCodeHasher.VerifyPairingCode(normalizedCode, candidate.CodeHash, candidate.Salt))
            {
                matchedCode = candidate;
                break;
            }
        }

        if (matchedCode == null)
        {
            // Ghi nhận failed attempt cho các mã gần đây nếu có dấu hiệu brute-force
            foreach (var candidate in candidateCodes.Where(c => !c.IsLockedOut(now)))
            {
                candidate.RecordFailedAttempt(now);
            }
            await _context.SaveChangesAsync(cancellationToken);

            throw new PairingCodeExpiredOrInvalidException();
        }

        var person = matchedCode.TrustedPerson;
        if (person == null || person.IsDeleted || person.Status == TrustedPersonStatus.Revoked)
        {
            throw new PairingCodeExpiredOrInvalidException("Người ủy thác tương ứng không còn tồn tại hoặc đã bị thu hồi.");
        }

        // Chặn tự ủy thác cho chính mình
        if (person.OwnerId == request.DelegateUserId)
        {
            throw new SelfDelegationProhibitedException();
        }

        // Đánh dấu đã ghép đôi
        matchedCode.MarkAsUsed(now);
        person.MarkAsPaired(request.DelegateUserId);

        // Lấy tên hiển thị của Owner
        var ownerUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == person.OwnerId, cancellationToken);
        var ownerDisplayName = ownerUser?.FullName ?? "Chủ tài sản";

        var auditLog = new ContinuityAuditLog(
            person.OwnerId,
            person.Id,
            "TRUSTED_PERSON_PAIRING_COMPLETED",
            $"{{\"delegateUserId\":\"{request.DelegateUserId}\",\"pairedAt\":\"{now:O}\"}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return new ClaimPairingResultDto(
            person.Id,
            person.OwnerId,
            ownerDisplayName,
            person.RoleDescription ?? person.Relationship,
            person.Status.ToString(),
            now);
    }
}
