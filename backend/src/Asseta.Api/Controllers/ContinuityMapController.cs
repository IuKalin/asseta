using Asseta.Application.Features.ContinuityMap.Services;
using Microsoft.AspNetCore.Mvc;

namespace Asseta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContinuityMapController : ControllerBase
{
    private readonly ContinuityMapService _continuityMapService;

    public ContinuityMapController(ContinuityMapService continuityMapService)
    {
        _continuityMapService = continuityMapService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? ownerId)
    {
        var targetOwnerId = ownerId ?? Guid.Parse("11111111-1111-1111-1111-111111111111");
        var result = await _continuityMapService.GetContinuityMapAsync(targetOwnerId);
        return Ok(new
        {
            success = true,
            data = result,
            meta = new
            {
                timestamp = DateTime.UtcNow,
                correlationId = Guid.NewGuid().ToString()
            }
        });
    }
}
