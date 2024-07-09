using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace PageTracker.Api.Controllers;

/// <summary>
/// Endpoints to manually check the health of the running service
/// </summary>
[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
public class HealthCheckController : ControllerBase
{
    /// <summary>
    /// Returns a message if the service is running
    /// </summary>
    [AllowAnonymous]
    [HttpGet(Name = "health")]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult HealthCheck()
    {
        string message = $"Running on {DateTime.Now} {TimeZoneInfo.Local.DisplayName}.";
        return Ok(message);
    }
}
