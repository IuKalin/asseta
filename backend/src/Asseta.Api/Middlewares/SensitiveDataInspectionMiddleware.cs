using System.Text.Json;
using Asseta.Application.Common.Security;

namespace Asseta.Api.Middlewares;

public class SensitiveDataInspectionMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly HashSet<string> ExcludedFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "cipherNotesBlob",
        "cipherNonce",
        "cipherAuthTag"
    };

    public SensitiveDataInspectionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method.ToUpperInvariant();
        if ((method == "POST" || method == "PUT" || method == "PATCH") &&
            context.Request.ContentType != null &&
            context.Request.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(
                context.Request.Body,
                encoding: System.Text.Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var bodyText = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (!string.IsNullOrWhiteSpace(bodyText))
            {
                InspectJsonText(bodyText);
            }
        }

        await _next(context);
    }

    private static void InspectJsonText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            InspectElement(doc.RootElement, "");
        }
        catch (JsonException)
        {
            // If body is malformed JSON, let the standard model binder handle bad request
        }
    }

    private static void InspectElement(JsonElement element, string currentPath)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    if (ExcludedFields.Contains(property.Name))
                        continue;

                    var propertyPath = string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}.{property.Name}";
                    InspectElement(property.Value, propertyPath);
                }
                break;

            case JsonValueKind.Array:
                int index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    InspectElement(item, $"{currentPath}[{index}]");
                    index++;
                }
                break;

            case JsonValueKind.String:
                var text = element.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    SensitiveDataInspector.EnsureSafe(text, currentPath);
                }
                break;
        }
    }
}
