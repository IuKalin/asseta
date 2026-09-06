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
}
