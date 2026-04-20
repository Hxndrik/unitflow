using Microsoft.AspNetCore.Mvc;

namespace Unitflow.Features.Health;

[ApiController]
[Route("health")]
public sealed class HealthController(IHealthService healthService) : ControllerBase
{
    [HttpGet]
    public ActionResult<HealthStatusDto> Get() => Ok(healthService.GetStatus());
}
