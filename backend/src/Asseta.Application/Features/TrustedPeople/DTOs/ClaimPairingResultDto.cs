namespace Asseta.Application.Features.TrustedPeople.DTOs;

public record ClaimPairingResultDto(
    Guid TrustedPersonId,
    Guid OwnerId,
    string OwnerDisplayName,
    string AssignedRole,
    string Status,
    DateTime PairedAtUtc);
