using System.Text.Json;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Application.Features.ContinuityMap.Queries.GetContinuityMap;
using Asseta.Domain.Entities;
using Asseta.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ContinuityMap.Commands.SubmitAssessment;

public class SubmitContinuityAssessmentCommandHandler : IRequestHandler<SubmitContinuityAssessmentCommand, AssessmentResultDto>
{
    private readonly IAssetaDbContext _context;
    private readonly IMediator _mediator;

    public SubmitContinuityAssessmentCommandHandler(IAssetaDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<AssessmentResultDto> Handle(SubmitContinuityAssessmentCommand request, CancellationToken cancellationToken)
    {
        var categories = await _context.ContinuityCategories.ToListAsync(cancellationToken);
        var createdItems = new List<ContinuityItem>();

        int sortOrder = 1;
        foreach (var answer in request.Answers.Where(a => a.HasItem && !string.IsNullOrWhiteSpace(a.ItemName)))
        {
            var category = categories.FirstOrDefault(c => 
                string.Equals(c.Code, answer.CategoryCode, StringComparison.OrdinalIgnoreCase))
                ?? categories.First();

            var item = new ContinuityItem(
                request.OwnerId,
                category.Id,
                answer.ItemName,
                answer.Priority,
                answer.DocumentLocationHint,
                null,
                null,
                sortOrder++);

            createdItems.Add(item);
            _context.ContinuityItems.Add(item);
        }

        // Calculate initial readiness score
        var itemsByCategory = categories.ToDictionary(
            c => c.Id,
            c => createdItems.Where(i => i.CategoryId == c.Id).AsEnumerable());
        var initialScore = ReadinessScoreCalculator.CalculateOverallScore(itemsByCategory);

        // Store raw assessment history
        var rawJson = JsonSerializer.Serialize(request.Answers);
        var history = new ContinuityAssessmentHistory(
            request.OwnerId,
            rawJson,
            createdItems.Count,
            initialScore.Value,
            request.AssessmentVersion);

        _context.ContinuityAssessmentHistories.Add(history);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            null,
            "ASSESSMENT_COMPLETED",
            $"{{\"itemsCreated\":{createdItems.Count},\"initialScore\":{initialScore.Value}}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        var mapDto = await _mediator.Send(new GetContinuityMapQuery(request.OwnerId), cancellationToken);

        return new AssessmentResultDto(
            history.Id,
            createdItems.Count,
            initialScore.Value,
            mapDto);
    }
}
