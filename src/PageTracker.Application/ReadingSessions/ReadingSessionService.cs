using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using PageTracker.Domain.Models;
using PageTracker.Infrastructure.Persistence;

namespace PageTracker.Application.ReadingSessions
{
    /// <summary>
    /// Methods related to recording and managing reading sessions
    /// </summary>
    public interface IReadingSessionService
    {
        /// <summary>
        /// Creates a reading session where you have read the provided number of pages on the current day
        /// </summary>
        /// <param name="numberOfPages">The number of full pages you've read. Must be 0 or more.</param>
        Task<ReadingSession> RecordPages(int numberOfPages, CancellationToken cancellationToken = default);
    }
    internal class ReadingSessionService(ILogger<ReadingSessionService> logger, IPageTrackerDbContext context, TimeProvider timeProvider) : IReadingSessionService
    {
        private const int MininumNumberOfPages = 0;

        public async Task<ReadingSession> RecordPages(int numberOfPages, CancellationToken cancellationToken = default)
        {
            // Validate
            Guard.IsGreaterThanOrEqualTo(numberOfPages, MininumNumberOfPages, nameof(numberOfPages));

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
                return readingSession;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while saving reading session");
                throw;
            }
        }
    }
}
