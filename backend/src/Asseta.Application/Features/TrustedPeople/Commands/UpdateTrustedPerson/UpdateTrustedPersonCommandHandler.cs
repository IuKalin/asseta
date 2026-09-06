using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.TrustedPeople.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.TrustedPeople.Commands.UpdateTrustedPerson;

public class UpdateTrustedPersonCommandHandler : IRequestHandler<UpdateTrustedPersonCommand, TrustedPersonDto>
{
    private readonly IAssetaDbContext _context;

    public UpdateTrustedPersonCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<TrustedPersonDto> Handle(UpdateTrustedPersonCommand request, CancellationToken cancellationToken)
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

        if (person.RowVersion != request.ExpectedRowVersion)
        {
            throw new ConcurrencyException("Dữ liệu Người Ủy Thác đã bị sửa đổi bởi một tiến trình khác. Vui lòng tải lại.");
        }

        // Chặn trùng lặp SĐT hoặc Email với các người ủy thác khác của Owner
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedPhone = request.PhoneNumber.Trim();

        var duplicateExists = await _context.TrustedPeople
            .AnyAsync(p => p.OwnerId == request.OwnerId &&
                           p.Id != request.Id &&
                           !p.IsDeleted &&
                           (p.Email.ToLower() == normalizedEmail || p.PhoneNumber == normalizedPhone),
                      cancellationToken);

        if (duplicateExists)
        {
            throw new DuplicateContactException();
        }

        // [REQ-TRUST-024]: Nếu hạ xuống Level 1 (Notice Only), tự động thu hồi toàn bộ phân quyền danh mục
        if (request.TrustLevel == 1 && person.TrustLevel > 1)
        {
            var existingPermissions = await _context.TrustedPersonPermissions
                .Where(p => p.TrustedPersonId == person.Id && !p.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var perm in existingPermissions)
            {
                perm.MarkDeleted();
            }
        }

        person.UpdateProfile(
            request.FullName.Trim(),
            request.Email.Trim(),
            request.PhoneNumber.Trim(),
            request.Relationship.Trim(),
            request.RoleDescription?.Trim(),
            request.TrustLevel);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            person.Id,
            "TRUSTED_PERSON_UPDATED",
            $"{{\"fullName\":\"{person.FullName}\",\"email\":\"{person.Email}\",\"trustLevel\":{person.TrustLevel}}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return TrustedPersonDto.FromEntity(person);
    }
}
