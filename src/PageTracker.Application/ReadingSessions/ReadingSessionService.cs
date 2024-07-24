using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
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
        /// Returns the number of pages read on the given day. <br/>
        /// Given day starts from the 12am on the day provided by the offset timezone. 
        /// </summary>
        /// <param name="dateToRetrieve">Date where the pages should be counted on.</param>
        /// <returns>The total number of pages read</returns>
        Task<int> GetNumberOfPagesRead(DateTimeOffset dateToRetrieve, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves the page number that the reader was on when they finished their reading session and then calculates<br/>
        /// the number of pages read since the last recorded session.
        /// </summary>
        /// <param name="pageNumber">The page number the reader was on when they finished their reading session</param>
        Task<ReadingSession> RecordFinishedAt(int pageNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a reading session where the reader has read the provided number of pages on the current day
        /// </summary>
        /// <param name="numberOfPages">The number of full pages the reader has read. Must be 0 or more.</param>
        Task<ReadingSession> RecordPages(int numberOfPages, CancellationToken cancellationToken = default);
    }

    internal class ReadingSessionService(ILogger<ReadingSessionService> logger, IPageTrackerDbContext context, TimeProvider timeProvider) : IReadingSessionService
    {
        private const int MininumNumberOfPages = 0;
        private const int DefaultStartingPage = 1;

        public async Task<int> GetNumberOfPagesRead(DateTimeOffset dateToRetrieve, CancellationToken cancellationToken = default)
        {
            var dayStartInclusive = new DateTimeOffset(dateToRetrieve.Date, dateToRetrieve.Offset);
            var dayEndExclusive = dayStartInclusive.AddDays(1);

            var readingSessions = await context.ReadingSessions
                .Where(x => x.DateOfSession >= dayStartInclusive && x.DateOfSession < dayEndExclusive)
                .ToListAsync(cancellationToken);

            string message = $"{nameof(GetNumberOfPagesRead)} found {readingSessions.Count} reading sessions recorded from {dayStartInclusive} to {dayEndExclusive}.";
            logger.LogInformation(message);

            return readingSessions.Sum(x => x.NumberOfPages);
        }

        public async Task<ReadingSession> RecordFinishedAt(int pageNumber, CancellationToken cancellationToken = default)
        {
            Guard.IsGreaterThanOrEqualTo(pageNumber, DefaultStartingPage);

            // Fetch the most recent reading session
            var latestReadingSession = await context.ReadingSessions.OrderByDescending(x => x.DateOfSession).FirstOrDefaultAsync();
            var previousPageNumber = latestReadingSession?.PageFinishedOn ?? DefaultStartingPage;

            Guard.IsGreaterThanOrEqualTo(pageNumber, previousPageNumber);

            // Calculate the number of read pages
            int numPagesRead = pageNumber - previousPageNumber;

            // Create the reading session
            var readingSession = new ReadingSession
            {
                NumberOfPages = numPagesRead,
                PageFinishedOn = pageNumber,
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
                logger.LogError(ex, $"{nameof(RecordFinishedAt)}: Error occurred while saving reading session");
                throw;
            }
        }

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
                logger.LogError(ex, $"{nameof(RecordPages)}: Error occurred while saving reading session");
                throw;
            }
        }
    }
}
