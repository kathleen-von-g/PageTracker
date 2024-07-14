using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using NSubstitute.ReceivedExtensions;
using PageTracker.Application.ReadingSessions;
using PageTracker.Application.Tests.Common;
using PageTracker.Common.Extensions;
using PageTracker.Domain.Models;
using PageTracker.Infrastructure.Persistence;
using Xunit.Abstractions;

namespace PageTracker.Application.Tests.ReadingSessions;
public class ReadingSessionServiceTests
{
    private readonly ITestOutputHelper _output;
    private readonly ILogger<ReadingSessionService> _logger;
    
    public ReadingSessionServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _logger = new TestOutputLogger<ReadingSessionService>(output);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(-30)]
    public async Task RecordPages_Trows_ArgumentException_NumberOfPages_IsLessThan0(int numberOfPages)
    {
        // Arrange
        var service = new ReadingSessionService(_logger, Substitute.For<IPageTrackerDbContext>(), TimeProvider.System);

        // Act
        var actualException = await Record.ExceptionAsync(() => service.RecordPages(numberOfPages));

        // Assert
        actualException.ShouldNotBeNull();
        actualException.ShouldBeAssignableTo<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(22)]
    public async Task RecordPages_SavesToDatabase(int numberOfPages)
    {
        // Arrange
        var now = DateTimeOffset.Parse("2024-07-11 10:30:00+10:00");
        var timeProvider = new FakeTimeProvider(now);
        var mockSet = Substitute.For<DbSet<ReadingSession>>();
        var mockContext = Substitute.For<IPageTrackerDbContext>();
        mockContext.ReadingSessions.Returns(mockSet);
        var service = new ReadingSessionService(_logger, mockContext, timeProvider);

        // Act
        var readingSession = await service.RecordPages(numberOfPages);
        _output.WriteLine(readingSession.Serialize());

        // Assert
        readingSession.NumberOfPages.ShouldBe(numberOfPages);
        readingSession.DateOfSession.ShouldBe(now);

        mockSet.Received(1).Add(Arg.Is<ReadingSession>(r => r.NumberOfPages == numberOfPages && r.DateOfSession == now && r.ID == 0));
        await mockContext.Received(1).SaveChangesAsync();
    }
}
