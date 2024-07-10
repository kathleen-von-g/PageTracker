using Microsoft.EntityFrameworkCore;
using PageTracker.Domain.Models;

namespace PageTracker.Infrastructure.Persistence;

public partial class PageTrackerDbContext
{
    public DbSet<ReadingSession> ReadingSessions { get; set; }
}
