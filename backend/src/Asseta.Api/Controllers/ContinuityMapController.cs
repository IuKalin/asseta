using Asseta.Api.Models;
using Asseta.Application.Features.ContinuityMap.Commands.SubmitAssessment;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Application.Features.ContinuityMap.Queries.GetContinuityGaps;
using Asseta.Application.Features.ContinuityMap.Queries.GetContinuityMap;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Asseta.Api.Controllers;

[Route("api/v1/continuity-map")]
[Route("api/continuity-map")]
public class ContinuityMapController : BaseApiController
{
    private readonly IMediator _mediator;

    public ContinuityMapController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<ContinuityMapDto>>> Get([FromQuery] Guid? ownerId)
    {
        var targetOwnerId = ownerId ?? CurrentOwnerId;
        var result = await _mediator.Send(new GetContinuityMapQuery(targetOwnerId));
        return OkResponse(result);
    }

    [HttpGet("gaps")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ContinuityGapDto>>>> GetGaps([FromQuery] Guid? ownerId)
    {
        var targetOwnerId = ownerId ?? CurrentOwnerId;
        var result = await _mediator.Send(new GetContinuityGapsQuery(targetOwnerId));
        return OkResponse(result);
    }

    [HttpPost("assessment")]
    public async Task<ActionResult<ApiResponse<AssessmentResultDto>>> SubmitAssessment([FromBody] SubmitAssessmentRequest request)
    {
        var command = new SubmitContinuityAssessmentCommand(
            CurrentOwnerId,
            request.Answers,
            request.AssessmentVersion ?? "v1");

        var result = await _mediator.Send(command);
        return CreatedResponse("/api/v1/continuity-map", result);
    }
}

public record SubmitAssessmentRequest(
    List<AssessmentAnswerDto> Answers,
    string? AssessmentVersion = "v1");
