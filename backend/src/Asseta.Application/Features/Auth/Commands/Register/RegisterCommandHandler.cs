using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Common.Security;
using Asseta.Application.Features.Auth.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IAssetaDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(
        IAssetaDbContext context,
        IPasswordHasher _hasher,
        IJwtTokenService jwtTokenService,
        IEmailService emailService)
    {
        _context = context;
        _passwordHasher = _hasher;
        _jwtTokenService = jwtTokenService;
        _emailService = emailService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToLowerInvariant().Trim();

        var existingUser = await _context.Users
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (existingUser)
        {
            throw new ConflictException($"Email '{request.Email}' đã được đăng ký trên hệ thống.");
        }

        var masterKey = MasterKeyGenerator.GenerateMasterKey();
        var masterKeyVerifier = MasterKeyGenerator.ComputeVerifier(masterKey);
        var encryptionSalt = MasterKeyGenerator.GenerateSaltHex();
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User(
            normalizedEmail,
            passwordHash,
            request.FullName,
            masterKeyVerifier,
            encryptionSalt,
            request.PhoneNumber);

        _context.Users.Add(user);

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshTokenStr = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = _passwordHasher.HashPassword(refreshTokenStr);

        var refreshTokenEntity = new UserRefreshToken(
            user.Id,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(30));

        _context.UserRefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync(cancellationToken);

        // Send Master Key to User's Email
        await _emailService.SendMasterKeyEmailAsync(
            user.Email,
            user.FullName,
            masterKey,
            cancellationToken);

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
            userDto,
            masterKey);
    }
}
