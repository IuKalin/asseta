using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.Auth.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IAssetaDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        IAssetaDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Load active unrevoked tokens
        var tokens = await _context.UserRefreshTokens
            .Where(t => !t.IsRevoked && t.ExpiresAtUtc > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        UserRefreshToken? matchedToken = null;
        foreach (var token in tokens)
        {
            if (_passwordHasher.VerifyPassword(request.RefreshToken, token.TokenHash))
            {
                matchedToken = token;
                break;
            }
        }

        if (matchedToken == null)
        {
            throw new AuthException("Refresh token không hợp lệ hoặc đã hết hạn.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == matchedToken.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            throw new AuthException("Người dùng không tồn tại.");
        }

        // Revoke used token
        matchedToken.Revoke();

        // Issue new token pair
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshTokenStr = _jwtTokenService.GenerateRefreshToken();
        var newRefreshTokenHash = _passwordHasher.HashPassword(newRefreshTokenStr);

        var newRefreshTokenEntity = new UserRefreshToken(
            user.Id,
            newRefreshTokenHash,
            DateTime.UtcNow.AddDays(30));

        _context.UserRefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto(
            user.Id,
            user.Email,
            user.FullName,
            user.PhoneNumber,
            user.EncryptionSalt,
            user.Status.ToString(),
            user.CreatedAtUtc,
            user.MasterKeyVerifier);

        return new AuthResponseDto(
            newAccessToken,
            newRefreshTokenStr,
            3600,
            userDto);
    }
}
