using Asseta.Application.Features.ContinuityMap.DTOs;
using MediatR;

namespace Asseta.Application.Features.ContinuityMap.Commands.SubmitAssessment;

public record SubmitContinuityAssessmentCommand(
    Guid OwnerId,
    List<AssessmentAnswerDto> Answers,
    string AssessmentVersion = "v1") : IRequest<AssessmentResultDto>;
