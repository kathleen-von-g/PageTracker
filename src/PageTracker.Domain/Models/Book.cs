using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PageTracker.Domain.Models;

/// <summary>
/// A book that you can record reading sessions against
/// </summary>
public class Book
{
    /// <summary>
    /// Unique book reference
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Title of the book
    /// </summary>
    [MaxLength(1000)]
    public required string Title { get; set; }

    /// <summary>
    /// Author/s of the book. If more than one author, split with commas
    /// </summary>
    [MaxLength(200)]
    public required string Author { get; set; }

    /// <summary>
    /// The page number the actual readable text begins on (i.e. excluding table of context, forewards etc)
    /// </summary>
    public required int StartingPage { get; set; }

    /// <summary>
    /// The page number the actual readable text finishes on. <br/>
    /// If you intend to read the afterward, then that is considered ‘readable content’
    /// </summary>
    public required int EndingPage { get; set; }

    /// <summary>
    /// Reading sessions where this book was read
    /// </summary>
    [JsonIgnore]
    public ICollection<ReadingSession> ReadingSessions { get; set; } = null!;
}
