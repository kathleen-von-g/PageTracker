using Microsoft.EntityFrameworkCore;
using PageTracker.Domain.Models;

namespace PageTracker.Infrastructure.Persistence;

internal partial class PageTrackerDbContext
{
    public virtual DbSet<Book> Books { get; set; }
    public virtual DbSet<ReadingSession> ReadingSessions { get; set; }
}
