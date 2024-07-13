using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace PageTracker.Infrastructure.Persistence;

internal partial class PageTrackerDbContext : DbContext, IPageTrackerDbContext
{
    public PageTrackerDbContext(DbContextOptions<PageTrackerDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PageTrackerDbContext).Assembly);
    }
}
