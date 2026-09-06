namespace Asseta.Domain.Constants;

public static class CategoryCodes
{
    public const string Financial = "FINANCIAL";
    public const string Property = "PROPERTY";
    public const string Insurance = "INSURANCE";
    public const string Business = "BUSINESS";
    public const string Documents = "DOCUMENTS";
    public const string Family = "FAMILY";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Financial,
        Property,
        Insurance,
        Business,
        Documents,
        Family
    };

    public static readonly Guid FinancialId = Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e01");
    public static readonly Guid PropertyId  = Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e02");
    public static readonly Guid InsuranceId = Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e03");
    public static readonly Guid BusinessId  = Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e04");
    public static readonly Guid DocumentsId = Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e05");
    public static readonly Guid FamilyId    = Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e06");

    public static readonly Dictionary<Guid, Guid> LegacyIdMap = new()
    {
        // Repeating hex IDs (1111..., 2222..., 4444..., etc.)
        [Guid.Parse("11111111-1111-1111-1111-111111111111")] = FinancialId,
        [Guid.Parse("22222222-2222-2222-2222-222222222222")] = PropertyId,
        [Guid.Parse("33333333-3333-3333-3333-333333333333")] = InsuranceId,
        [Guid.Parse("44444444-4444-4444-4444-444444444444")] = BusinessId,
        [Guid.Parse("55555555-5555-5555-5555-555555555555")] = DocumentsId,
        [Guid.Parse("66666666-6666-6666-6666-666666666666")] = FamilyId,

        // Sequential mock format (1111...1111 to 1111...1116)
        [Guid.Parse("11111111-1111-1111-1111-111111111112")] = PropertyId,
        [Guid.Parse("11111111-1111-1111-1111-111111111113")] = InsuranceId,
        [Guid.Parse("11111111-1111-1111-1111-111111111114")] = BusinessId,
        [Guid.Parse("11111111-1111-1111-1111-111111111115")] = DocumentsId,
        [Guid.Parse("11111111-1111-1111-1111-111111111116")] = FamilyId,

        // Mobile mock format (c1111111... to c6666666...)
        [Guid.Parse("c1111111-1111-1111-1111-111111111111")] = FinancialId,
        [Guid.Parse("c2222222-2222-2222-2222-222222222222")] = PropertyId,
        [Guid.Parse("c3333333-3333-3333-3333-333333333333")] = InsuranceId,
        [Guid.Parse("c4444444-4444-4444-4444-444444444444")] = BusinessId,
        [Guid.Parse("c5555555-5555-5555-5555-555555555555")] = DocumentsId,
        [Guid.Parse("c6666666-6666-6666-6666-666666666666")] = FamilyId,
    };

    public static Guid ResolveCanonicalId(Guid categoryId)
    {
        return LegacyIdMap.TryGetValue(categoryId, out var canonical) ? canonical : categoryId;
    }
}
