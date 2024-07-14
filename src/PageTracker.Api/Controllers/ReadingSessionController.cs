using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PageTracker.Application.ReadingSessions;
using PageTracker.Domain.Models;

namespace PageTracker.Api.Controllers;

/// <summary>
/// Endpoints to record reading sessions you've done.
/// </summary>
[ApiController]
[Route("reading-session")]
[ApiVersion("1.0")]
public class ReadingSessionController(ILogger<ReadingSessionController> logger, IReadingSessionService readingSessionService) : ControllerBase
{
    private const string RecordPagesError = "Could not record pages";


    /// <summary>
    /// Creates a reading session where you have read the provided number of pages on the current day.
    /// </summary>
    /// <param name="numberOfPages">The number of full pages you've read. Must be 0 or more.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Multiple reading sessions can be made per day. <br/>
    /// Number of pages are further defined as:<br/>
    /// If I started on page 10 and ended up on page 20, I read 10 pages.<br/>
    /// If I started on page 46 and ended on page 46, I read 0 pages.<br/>
    /// If I started on page 1 and finished on the middle of page 2, then I read 1 FULL page.
    /// </remarks>
    /// <response code="201">Successfully created a reading session</response>
    /// <response code="400">If the number of pages was less than 0</response>
    [HttpPost("pages/{numberOfPages}")]
    [ProducesResponseType<ReadingSession>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> RecordPages([FromRoute]int numberOfPages, CancellationToken cancellationToken = default)
    {
        try
        {
            var readingSession = await readingSessionService.RecordPages(numberOfPages, cancellationToken);
            return Ok(readingSession);
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, RecordPagesError);
            return BadRequest(new ProblemDetails
            {
                Title = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, RecordPagesError);
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError, title: RecordPagesError);
        }
    }
}
