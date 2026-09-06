namespace Asseta.Application.Features.Auth.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string? PhoneNumber,
    string EncryptionSalt,
    string Status,
    DateTime CreatedAtUtc,
    string MasterKeyVerifier = "");

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresInSeconds,
    UserDto User,
    string? MasterKey = null);

public record RegisterRequestDto(
    string Email,
    string Password,
    string FullName,
    string? PhoneNumber);

public record LoginRequestDto(
    string Email,
    string Password);

public record RefreshTokenRequestDto(
    string RefreshToken);

public record VerifyMasterKeyRequestDto(
    string MasterKey);

public record VerifyMasterKeyResponseDto(
    bool IsValid,
    string EncryptionSalt);
