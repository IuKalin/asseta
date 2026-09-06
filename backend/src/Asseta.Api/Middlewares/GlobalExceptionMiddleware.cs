using System.Net;
using System.Text.Json;
using Asseta.Api.Models;
using Asseta.Application.Common.Exceptions;
using FluentValidation;

namespace Asseta.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() 
            ?? Guid.NewGuid().ToString();

        var (statusCode, errorCode, message, details) = exception switch
        {
            ValidationException valEx => (
                HttpStatusCode.BadRequest,
                "VALIDATION_FAILED",
                "Validation error occurred.",
                valEx.Errors.Select(e => new { field = e.PropertyName, issue = e.ErrorMessage })
            ),
            ArgumentException argEx => (
                HttpStatusCode.BadRequest,
                "VALIDATION_FAILED",
                argEx.Message,
                null
            ),
            SensitiveDataDetectedException sensEx => (
                HttpStatusCode.UnprocessableEntity,
                "SENSITIVE_DATA_DETECTED",
                sensEx.Message,
                (object)new { field = sensEx.FieldName, pattern = sensEx.PatternDetected }
            ),
            ConcurrencyException concEx => (
                HttpStatusCode.Conflict,
                "CONCURRENT_STATE_MUTATION",
                concEx.Message,
                null
            ),
            ConflictException confEx => (
                HttpStatusCode.Conflict,
                "RESOURCE_CONFLICT",
                confEx.Message,
                null
            ),
            AuthException authEx => (
                HttpStatusCode.Unauthorized,
                "AUTHENTICATION_FAILED",
                authEx.Message,
                null
            ),
            UnauthorizedResourceAccessException unauthEx => (
                HttpStatusCode.Forbidden,
                "UNAUTHORIZED_RESOURCE_ACCESS",
                unauthEx.Message,
                null
            ),
            ForbiddenAccessException forbEx => (
                HttpStatusCode.Forbidden,
                "INSUFFICIENT_TRUST_LEVEL_FOR_ACTIVATION",
                forbEx.Message,
                null
            ),
            InvalidOperationException invOpEx => (
                HttpStatusCode.BadRequest,
                "INVALID_OPERATION",
                invOpEx.Message,
                null
            ),
            PairingAttemptsExceededException pairLimitEx => (
                (HttpStatusCode)429,
                "PAIRING_ATTEMPTS_EXCEEDED",
                pairLimitEx.Message,
                null
            ),
            PairingCodeExpiredOrInvalidException pairExpEx => (
                HttpStatusCode.BadRequest,
                "PAIRING_CODE_EXPIRED_OR_INVALID",
                pairExpEx.Message,
                null
            ),
            DuplicateContactException dupEx => (
                HttpStatusCode.Conflict,
                "DUPLICATE_TRUSTED_PERSON_CONTACT",
                dupEx.Message,
                null
            ),
            MaxTrustedPeopleExceededException maxEx => (
                HttpStatusCode.BadRequest,
                "MAX_TRUSTED_PEOPLE_EXCEEDED",
                maxEx.Message,
                null
            ),
            SelfDelegationProhibitedException selfEx => (
                HttpStatusCode.BadRequest,
                "SELF_DELEGATION_PROHIBITED",
                selfEx.Message,
                null
            ),
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                "CONTINUITY_ITEM_NOT_FOUND",
                notFoundEx.Message,
                null
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "INTERNAL_SERVER_ERROR",
                "An unexpected server error occurred.",
                null
            )
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled error: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Handled domain exception [{Code}]: {Message}", errorCode, message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(errorCode, message, details, correlationId);
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
