using Asseta.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Common.Interfaces;

public interface IAssetaDbContext
{
    DbSet<ContinuityCategory> ContinuityCategories { get; }
    DbSet<ContinuityItem> ContinuityItems { get; }
    DbSet<ContinuityAuditLog> ContinuityAuditLogs { get; }
    DbSet<ContinuityAssessmentHistory> ContinuityAssessmentHistories { get; }
    DbSet<ActionCard> ActionCards { get; }
    DbSet<ActionCardStep> ActionCardSteps { get; }
    DbSet<ActionCardContact> ActionCardContacts { get; }
    DbSet<ActionCardTemplate> ActionCardTemplates { get; }
    DbSet<User> Users { get; }
    DbSet<UserRefreshToken> UserRefreshTokens { get; }
    DbSet<TrustedPerson> TrustedPeople { get; }
    DbSet<TrustedPersonPairingCode> TrustedPersonPairingCodes { get; }
    DbSet<TrustedPersonPermission> TrustedPersonPermissions { get; }
    DbSet<OwnerActivationConfig> OwnerActivationConfigs { get; }
    DbSet<ActivationRequest> ActivationRequests { get; }
    DbSet<ActivationConfirmation> ActivationConfirmations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
