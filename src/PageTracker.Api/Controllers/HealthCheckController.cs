using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace PageTracker.Api.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
public class HealthCheckController : ControllerBase
{
}
