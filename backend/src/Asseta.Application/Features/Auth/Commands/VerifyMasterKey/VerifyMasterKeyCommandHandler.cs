using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Common.Security;
using Asseta.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.Auth.Commands.VerifyMasterKey;

public class VerifyMasterKeyCommandHandler : IRequestHandler<VerifyMasterKeyCommand, VerifyMasterKeyResponseDto>
{
    private readonly IAssetaDbContext _context;

    public VerifyMasterKeyCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<VerifyMasterKeyResponseDto> Handle(VerifyMasterKeyCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User", request.UserId);
        }

        var normalizedKey = request.MasterKey?.Trim().ToUpperInvariant() ?? string.Empty;
        if (string.IsNullOrEmpty(normalizedKey))
        {
            throw new AuthException("Master Key không được để trống.");
        }

        var computedVerifier = MasterKeyGenerator.ComputeVerifier(normalizedKey);

        if (!string.Equals(computedVerifier, user.MasterKeyVerifier, StringComparison.OrdinalIgnoreCase))
        {
            throw new AuthException("Master Key không chính xác. Vui lòng nhập đúng mã khóa bảo mật được cấp khi đăng ký tài khoản (định dạng AK-XXXX-XXXX-XXXX-XXXX hoặc khóa Demo: AK-DEMO-2026-ASSETA-VAULT).");
        }

        return new VerifyMasterKeyResponseDto(true, user.EncryptionSalt);
    }
}
