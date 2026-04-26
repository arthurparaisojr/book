using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Book.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        return BuildHealthResponseAsync(_ => true, cancellationToken);
    }

    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public Task<IActionResult> GetLive(CancellationToken cancellationToken)
    {
        return BuildHealthResponseAsync(check => check.Tags.Contains("live"), cancellationToken);
    }

    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public Task<IActionResult> GetReady(CancellationToken cancellationToken)
    {
        return BuildHealthResponseAsync(check => check.Tags.Contains("ready"), cancellationToken);
    }

    private async Task<IActionResult> BuildHealthResponseAsync(
        Func<HealthCheckRegistration, bool> predicate,
        CancellationToken cancellationToken)
    {
        var report = await _healthCheckService.CheckHealthAsync(predicate, cancellationToken);
        var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        var response = new
        {
            service = "book-api",
            status = report.Status.ToString().ToLowerInvariant(),
            utcNow = DateTime.UtcNow,
            traceId,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString().ToLowerInvariant(),
                description = entry.Value.Description
            })
        };

        if (report.Status == HealthStatus.Healthy)
        {
            return Ok(response);
        }

        return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}
