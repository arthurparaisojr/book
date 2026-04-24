using Microsoft.AspNetCore.Mvc;

namespace Book.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            service = "book-api",
            status = "healthy",
            utcNow = DateTime.UtcNow
        });
    }
}
