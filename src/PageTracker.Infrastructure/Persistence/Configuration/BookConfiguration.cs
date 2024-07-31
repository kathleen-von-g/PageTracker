using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PageTracker.Domain.Models;

namespace PageTracker.Infrastructure.Persistence.Configuration;

internal class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(e => e.ID);

        builder.HasMany(p => p.ReadingSessions)
            .WithOne(d => d.Book)
            .HasForeignKey(d => d.BookID);
    }
}
