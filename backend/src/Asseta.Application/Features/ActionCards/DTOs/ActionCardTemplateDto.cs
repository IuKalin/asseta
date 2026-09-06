using System.Text.Json;
using Asseta.Domain.Entities;

namespace Asseta.Application.Features.ActionCards.DTOs;

public record TemplateStepDto(int StepOrder, string Instruction, string? EstimatedDuration);

public record ActionCardTemplateDto(
    Guid Id,
    string TemplateCode,
    string CategoryCode,
    string TitleVi,
    string TitleEn,
    string DefaultUrgency,
    string DefaultPriority,
    IReadOnlyList<TemplateStepDto> SuggestedSteps,
    IReadOnlyList<string> SuggestedRoles)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static ActionCardTemplateDto FromEntity(ActionCardTemplate template)
    {
        var steps = string.IsNullOrWhiteSpace(template.SuggestedStepsJson)
            ? new List<TemplateStepDto>()
            : JsonSerializer.Deserialize<List<TemplateStepDto>>(template.SuggestedStepsJson, JsonOptions) ?? new List<TemplateStepDto>();

        var roles = string.IsNullOrWhiteSpace(template.SuggestedRolesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(template.SuggestedRolesJson, JsonOptions) ?? new List<string>();

        return new ActionCardTemplateDto(
            template.Id,
            template.TemplateCode,
            template.CategoryCode,
            template.TitleVi,
            template.TitleEn,
            template.DefaultUrgency,
            template.DefaultPriority,
            steps,
            roles);
    }
}
