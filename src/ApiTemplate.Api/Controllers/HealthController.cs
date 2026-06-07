namespace ApiTemplate.Api.Controllers;

/// <summary>Provides a simple liveness check endpoint.</summary>
[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    /// <summary>Returns 200 OK to confirm the API process is alive.</summary>
    [HttpGet("live")]
    [AllowAnonymous]
    public IActionResult Live() => Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
}
