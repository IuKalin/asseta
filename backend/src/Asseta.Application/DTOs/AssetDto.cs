using Asseta.Domain.Enums;

namespace Asseta.Application.DTOs;

public record AssetDto(
    Guid Id,
    string Name,
    string Description,
    AssetType Type,
    decimal EstimatedValue,
    DateTime CreatedAtUtc
);
