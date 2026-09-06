using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.Auth.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAssetaDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IAssetaDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToLowerInvariant().Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail && !u.IsDeleted, cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new AuthException("Email hoặc mật khẩu không chính xác.");
        }

        if (user.Status == UserStatus.LOCKED)
        {
            throw new UnauthorizedResourceAccessException("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshTokenStr = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = _passwordHasher.HashPassword(refreshTokenStr);

        var refreshTokenEntity = new UserRefreshToken(
            user.Id,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(30));

        _context.UserRefreshTokens.Add(refreshTokenEntity);
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
            accessToken,
            refreshTokenStr,
            3600,
            userDto);
    }
}
