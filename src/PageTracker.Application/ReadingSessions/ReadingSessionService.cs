using Microsoft.Extensions.Logging;
using PageTracker.Domain.Models;
using PageTracker.Infrastructure.Persistence;

namespace PageTracker.Application.ReadingSessions
{
    public interface IReadingSessionService
    {
        /// <summary>
        /// Creates a reading session where you have read the provided number of pages on the current day
        /// </summary>
        /// <param name="numberOfPages">The number of full pages you've read. Must be 0 or more.</param>
        Task RecordPages(int numberOfPages, CancellationToken cancellationToken = default);
    }
    public class ReadingSessionService(ILogger<ReadingSessionService> logger, IPageTrackerDbContext context, TimeProvider timeProvider) : IReadingSessionService
    {
        private const int MininumNumberOfPages = 0;

        public async Task RecordPages(int numberOfPages, CancellationToken cancellationToken = default)
        {
            // Validate
            if (numberOfPages < MininumNumberOfPages)
            {
                throw new ArgumentException($"Must be greater than {MininumNumberOfPages}", nameof(numberOfPages));
            }

            // Create reading session
            var readingSession = new ReadingSession
            {
                NumberOfPages = numberOfPages,
                DateOfSession = timeProvider.GetLocalNow()
            };

            try
            {
                // Add reading session
                context.ReadingSessions.Add(readingSession);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while saving reading session");
                throw;
            }
        }
    }
}
