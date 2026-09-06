using System.Text;
using Asseta.Application.Common.Interfaces;

namespace Asseta.Api.Middlewares;

public class IdempotencyMiddleware
{
    private const string IdempotencyHeader = "Idempotency-Key";
    private readonly RequestDelegate _next;

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IIdempotencyService idempotencyService)
    {
        var method = context.Request.Method.ToUpperInvariant();
        if (method != "POST" && method != "PUT" && method != "DELETE")
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(IdempotencyHeader, out var headerValue) ||
            string.IsNullOrWhiteSpace(headerValue))
        {
            await _next(context);
            return;
        }

        var idempotencyKey = headerValue.ToString();

        // 1. Check existing cached response
        var cachedResponse = await idempotencyService.GetResponseAsync(idempotencyKey);
        if (cachedResponse != null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync(cachedResponse);
            return;
        }

        // 2. Try acquiring lock
        var lockAcquired = await idempotencyService.TryAcquireLockAsync(idempotencyKey, TimeSpan.FromSeconds(15));
        if (!lockAcquired)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsync("{\"success\":false,\"error\":{\"code\":\"IDEMPOTENT_OPERATION_IN_PROGRESS\",\"message\":\"A request with the same Idempotency-Key is currently being processed.\"}}");
            return;
        }

        var originalBodyStream = context.Response.Body;
        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);

            // Cache 2xx responses for 24 hours
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                await idempotencyService.SaveResponseAsync(idempotencyKey, responseText, TimeSpan.FromHours(24));
            }

            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
            await idempotencyService.ReleaseLockAsync(idempotencyKey);
        }
    }
}
