namespace PageTracker.Domain.Models;

/// <summary>
/// A session where you have read a book. Stores information like "the number of pages read" and
/// "when the session was".
/// </summary>
public class ReadingSession
{
    /// <summary>
    /// Unique ID of this record
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// The number of full pages read in this session.
    /// </summary>
    /// <remarks>
    /// If I started on page 46 and ended on page 46, I read 0 pages.<br/>
    /// If I started on page 1 and finished on the middle of page 2, then I read 1 FULL page
    /// </remarks>
    public required int NumberOfPages { get; set; }

    /// <summary>
    /// Date and time this reading session was finished with time zone information. <br/>
    /// By default this is set to the date and time the session was recorded.
    /// </summary>
    public required DateTimeOffset DateOfSession { get; set; }

    /// <summary>
    /// The page number that the reader was on when they finished their reading session<br/>
    /// i.e. "I got up to page 45 today", 45 being the page they finished on
    /// </summary> 
    public required int PageFinishedOn { get; set; }

    /// <summary>
    /// The ID of the book that was read in this session
    /// </summary>
    public int? BookID { get; set; }

    /// <summary>
    /// The book that was read in this session
    /// </summary>
    public virtual Book? Book { get; set; }
}
