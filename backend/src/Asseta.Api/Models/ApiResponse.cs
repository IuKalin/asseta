namespace Asseta.Api.Models;

public record ApiError(string Code, string Message, object? Details = null);

public record ApiMeta(DateTime Timestamp, string CorrelationId);

public record ApiResponse<T>(
    bool Success,
    T? Data,
    ApiError? Error,
    ApiMeta Meta)
{
    public static ApiResponse<T> Ok(T data, string? correlationId = null) =>
        new(true, data, null, new ApiMeta(DateTime.UtcNow, correlationId ?? Guid.NewGuid().ToString()));

    public static ApiResponse<T> Fail(string code, string message, object? details = null, string? correlationId = null) =>
        new(false, default, new ApiError(code, message, details), new ApiMeta(DateTime.UtcNow, correlationId ?? Guid.NewGuid().ToString()));
}
