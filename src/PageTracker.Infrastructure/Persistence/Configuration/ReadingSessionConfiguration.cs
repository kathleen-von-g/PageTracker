using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PageTracker.Domain.Models;

namespace PageTracker.Infrastructure.Persistence.Configuration;
internal class ReadingSessionConfiguration : IEntityTypeConfiguration<ReadingSession>
{
    public void Configure(EntityTypeBuilder<ReadingSession> builder)
    {
        builder.HasKey(e => e.ID);

        builder.Property(e => e.PageFinishedOn).HasDefaultValue(1);
    }
}
