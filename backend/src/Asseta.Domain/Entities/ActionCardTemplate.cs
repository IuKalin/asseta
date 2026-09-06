using Asseta.Domain.Common;

namespace Asseta.Domain.Entities;

public class ActionCardTemplate : BaseEntity
{
    public string TemplateCode { get; private set; } = string.Empty;
    public string CategoryCode { get; private set; } = string.Empty;
    public string TitleVi { get; private set; } = string.Empty;
    public string TitleEn { get; private set; } = string.Empty;
    public string DefaultUrgency { get; private set; } = "FIRST_72_HOURS";
    public string DefaultPriority { get; private set; } = "CRITICAL";
    public string SuggestedStepsJson { get; private set; } = "[]";
    public string SuggestedRolesJson { get; private set; } = "[]";

    private ActionCardTemplate() { }

    public ActionCardTemplate(
        Guid id,
        string templateCode,
        string categoryCode,
        string titleVi,
        string titleEn,
        string defaultUrgency,
        string defaultPriority,
        string suggestedStepsJson,
        string suggestedRolesJson)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        TemplateCode = templateCode.Trim().ToUpperInvariant();
        CategoryCode = categoryCode.Trim().ToUpperInvariant();
        TitleVi = titleVi.Trim();
        TitleEn = titleEn.Trim();
        DefaultUrgency = defaultUrgency.Trim().ToUpperInvariant();
        DefaultPriority = defaultPriority.Trim().ToUpperInvariant();
        SuggestedStepsJson = suggestedStepsJson;
        SuggestedRolesJson = suggestedRolesJson;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
