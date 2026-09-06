using Asseta.Domain.Common;

namespace Asseta.Domain.Entities;

public class ContinuityCategory : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public string NameVi { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public string Icon { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }

    private readonly List<ContinuityItem> _items = new();
    public IReadOnlyCollection<ContinuityItem> Items => _items.AsReadOnly();

    private ContinuityCategory() { }

    public ContinuityCategory(Guid id, string code, string nameVi, string nameEn, string icon, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Category code cannot be empty.", nameof(code));
        if (string.IsNullOrWhiteSpace(nameVi))
            throw new ArgumentException("Vietnamese name cannot be empty.", nameof(nameVi));

        Id = id;
        Code = code.Trim().ToUpperInvariant();
        NameVi = nameVi.Trim();
        NameEn = nameEn.Trim();
        Icon = icon.Trim();
        SortOrder = sortOrder;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
