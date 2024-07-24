using Asp.Versioning;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PageTracker.Application.ReadingSessions;
using PageTracker.Domain.Models;
using System.Runtime.CompilerServices;

namespace PageTracker.Api.Controllers;

/// <summary>
/// Endpoints to record reading sessions you've done.
/// </summary>
[ApiController]
[Route("reading-session")]
[ApiVersion("1.0")]
public class ReadingSessionController(ILogger<ReadingSessionController> logger, IReadingSessionService readingSessionService, TimeProvider timeProvider) : ControllerBase
{
    private const string RecordPagesError = "Could not record the number of pages you read.";
    private const string GetPagesError = "Could not retrieve the number of pages you've read today.";
    private const string RecordPageFinishedAtError = "Could not record the page you finished at.";


    /// <summary>
    /// Creates a reading session where you have read the provided number of pages on the current day.<br/>
    /// Based on the page they reader finished on the last session, the current page is also calculated.
    /// </summary>
    /// <param name="numberOfPages">The number of full pages you've read. Must be 0 or more.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Multiple reading sessions can be made per day. <br/>
    /// Number of pages are further defined as:<br/>
    /// If I started on page 10 and read 10 pages, I finished on page 20.<br/>
    /// If I started on page 46 and read 0 pages, I finished on page 46.<br/>
    /// If I started on page 1 and read 1 page, I finished anywhere on page 2
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
            return Created(new Uri(Request.GetEncodedUrl() + "/" + readingSession.ID), readingSession);
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

    /// <summary>
    /// Returns the number of pages read today
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">The number of pages read today. Will be 0 if no pages were record</response>
    [HttpGet("pages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetRecordedPages(CancellationToken cancellationToken = default)
    {
        try
        {
            var numberOfPages = await readingSessionService.GetNumberOfPagesRead(timeProvider.GetLocalNow());
            return Ok(numberOfPages);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, GetPagesError);
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError, title: GetPagesError);
        }
    }

    /// <summary>
    /// Saves the page number that the reader was on when they finished their reading session and then calculates<br/>
    /// the number of pages read since the last recorded session.<br/>
    /// </summary>
    /// <param name="pageNumber">The page number the reader was on when they finished their reading session</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created reading session object</returns>
    /// <remarks>
    /// i.e. "I got up to page 45 today", 45 being the page they finished on. If they were previously on page 40, then
    /// they have read 5 pages today.<br/>
    /// Number of pages are further defined as:<br/>
    /// If I started on page 10 and ended up on page 20, I read 10 pages.<br/>
    /// If I started on page 46 and ended on page 46, I read 0 pages.<br/>
    /// If I started on page 1 and finished on the middle of page 2, then I read 1 FULL page.
    /// </remarks>
    /// <response code="201">Successfully created a reading session</response>
    /// <response code="400">If the page number was less than the last recorded page number, or less than the book's starting page.</response>
    [HttpPost("finished-at/{pageNumber}")]
    [ProducesResponseType<ReadingSession>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> RecordPageFinishedAt(int pageNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var readingSession = await readingSessionService.RecordPageFinishedAt(pageNumber, cancellationToken);
            return Created(new Uri(Request.GetEncodedUrl() + "/" + readingSession.ID), readingSession);
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
