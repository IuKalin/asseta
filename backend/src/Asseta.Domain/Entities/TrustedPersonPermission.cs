using Asseta.Domain.Common;
using Asseta.Domain.Enums;

namespace Asseta.Domain.Entities;

public class TrustedPersonPermission : BaseEntity
{
    public Guid TrustedPersonId { get; private set; }
    public PermissionType PermissionType { get; private set; }
    public Guid? TargetCategoryId { get; private set; }
    public Guid? TargetActionCardId { get; private set; }
    public bool CanView { get; private set; } = true;

    // Navigation properties
    public TrustedPerson? TrustedPerson { get; private set; }
    public ContinuityCategory? TargetCategory { get; private set; }
    public ActionCard? TargetActionCard { get; private set; }

    private TrustedPersonPermission() { }

    public static TrustedPersonPermission CreateForCategory(Guid trustedPersonId, Guid targetCategoryId, bool canView = true)
    {
        return new TrustedPersonPermission
        {
            TrustedPersonId = trustedPersonId,
            PermissionType = PermissionType.Category,
            TargetCategoryId = targetCategoryId,
            TargetActionCardId = null,
            CanView = canView
        };
    }

    public static TrustedPersonPermission CreateForActionCard(Guid trustedPersonId, Guid targetActionCardId, bool canView = true)
    {
        return new TrustedPersonPermission
        {
            TrustedPersonId = trustedPersonId,
            PermissionType = PermissionType.ActionCard,
            TargetCategoryId = null,
            TargetActionCardId = targetActionCardId,
            CanView = canView
        };
    }

    public void UpdateCanView(bool canView)
    {
        CanView = canView;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
