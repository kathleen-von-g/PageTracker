using Microsoft.EntityFrameworkCore;
using PageTracker.Domain.Models;

namespace PageTracker.Infrastructure.Persistence;
public interface IPageTrackerDbContext
{
    /// <summary>
    /// All saved books
    /// </summary>
    DbSet<Book> Books { get; }

    /// <summary>
    /// All recorded ReadingSessions
    /// </summary>
    DbSet<ReadingSession> ReadingSessions { get; }

    /// <summary>
    /// Persist unit of work changes to the backing store <see cref="DbContext.SaveChangesAsync(CancellationToken)"/>
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
