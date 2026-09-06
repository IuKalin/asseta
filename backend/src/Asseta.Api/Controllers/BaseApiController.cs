using System.Security.Claims;
using Asseta.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Asseta.Api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected Guid CurrentOwnerId
    {
        get
        {
            if (Request.Headers.TryGetValue("X-Owner-Id", out var headerVal) && Guid.TryParse(headerVal, out var headerGuid))
                return headerGuid;
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(sub, out var claimGuid))
                return claimGuid;
            return Guid.Parse("11111111-1111-1111-1111-111111111111");
        }
    }

    protected string CorrelationId =>
        Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();

    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data) =>
        Ok(ApiResponse<T>.Ok(data, CorrelationId));

    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(string uri, T data) =>
        Created(uri, ApiResponse<T>.Ok(data, CorrelationId));
}
