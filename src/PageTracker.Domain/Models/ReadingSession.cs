namespace PageTracker.Domain.Models;

/// <summary>
/// A session where you have read a book. Stores information like "the number of pages read" and
/// "when the session was"
/// </summary>
public class ReadingSession
{
    /// <summary>
    /// Unique ID of this record
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// The number of full pages read in this session
    /// </summary>
    /// <remarks>
    /// If I started on page 46 and ended on page 46, I read 0 pages.<br/>
    /// If I started on page 1 and finished on the middle of page 2, then I read 1 FULL page
    /// </remarks>
    public int NumberOfPages { get; set; }

    /// <summary>
    /// Date and time this reading session was finishe with time zone information. <br/>
    /// By default, set to the date and time this session was recorded.
    /// </summary>
    public DateTimeOffset DateOfSession { get; set; }
}
