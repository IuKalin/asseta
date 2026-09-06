using System.Reflection;
using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Infrastructure.Persistence;

public class AssetaDbContext : DbContext, IAssetaDbContext
{
    public DbSet<ContinuityCategory> ContinuityCategories => Set<ContinuityCategory>();
    public DbSet<ContinuityItem> ContinuityItems => Set<ContinuityItem>();
    public DbSet<ContinuityAuditLog> ContinuityAuditLogs => Set<ContinuityAuditLog>();
    public DbSet<ContinuityAssessmentHistory> ContinuityAssessmentHistories => Set<ContinuityAssessmentHistory>();
    public DbSet<ActionCard> ActionCards => Set<ActionCard>();
    public DbSet<ActionCardStep> ActionCardSteps => Set<ActionCardStep>();
    public DbSet<ActionCardContact> ActionCardContacts => Set<ActionCardContact>();
    public DbSet<ActionCardTemplate> ActionCardTemplates => Set<ActionCardTemplate>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
    public DbSet<TrustedPerson> TrustedPeople => Set<TrustedPerson>();
    public DbSet<TrustedPersonPairingCode> TrustedPersonPairingCodes => Set<TrustedPersonPairingCode>();
    public DbSet<TrustedPersonPermission> TrustedPersonPermissions => Set<TrustedPersonPermission>();
    public DbSet<OwnerActivationConfig> OwnerActivationConfigs => Set<OwnerActivationConfig>();
    public DbSet<ActivationRequest> ActivationRequests => Set<ActivationRequest>();
    public DbSet<ActivationConfirmation> ActivationConfirmations => Set<ActivationConfirmation>();

    public AssetaDbContext(DbContextOptions<AssetaDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
