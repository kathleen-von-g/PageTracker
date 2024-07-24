using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using MockQueryable.NSubstitute;
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

    private static IPageTrackerDbContext GetMockContext(List<ReadingSession> existingEntries)
    {
        var mockSet = existingEntries.AsQueryable().BuildMockDbSet();
        var mockContext = Substitute.For<IPageTrackerDbContext>();
        mockContext.ReadingSessions.Returns(mockSet);
        return mockContext;
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

    [Fact]
    public async Task GetNumberOfPagesRead_MultipleEntries()
    {
        // Arrange
        var now = DateTimeOffset.Parse("2024-07-11 23:00:00 +10:00");
        var timeProvider = new FakeTimeProvider(now);

        List<ReadingSession> existingEntries = [
            new ReadingSession { NumberOfPages = 1, DateOfSession = DateTimeOffset.Parse("2024-07-10 23:00:00 +10:00") }, // Yesterday
            new ReadingSession { NumberOfPages = 2, DateOfSession = DateTimeOffset.Parse("2024-07-11 8:20:00 +10:00") }, // Today
            new ReadingSession { NumberOfPages = 4, DateOfSession = DateTimeOffset.Parse("2024-07-11 13:50:00 +10:00") }, // Today
            new ReadingSession { NumberOfPages = 8, DateOfSession = DateTimeOffset.Parse("2024-07-11 18:36:00 +10:00") }, // Today
            ];

        var mockContext = GetMockContext(existingEntries);
        var service = new ReadingSessionService(_logger, mockContext, timeProvider);

        // Act
        var numberOfPagesRead = await service.GetNumberOfPagesRead(now);

        // Assert
        numberOfPagesRead.ShouldBe(14);
    }

    [Fact]
    public async Task GetNumberOfPagesRead_12AM_and_Midnight()
    {
        var queryDate = DateTimeOffset.Parse("2024-07-11 00:00:00 +10:00");
        var timeProvider = new FakeTimeProvider(queryDate);

        List<ReadingSession> existingEntries = [
            new ReadingSession { NumberOfPages = 1, DateOfSession = DateTimeOffset.Parse("2024-07-10 23:59:59 +10:00") }, // Midnight day before
            new ReadingSession { NumberOfPages = 2, DateOfSession = DateTimeOffset.Parse("2024-07-11 00:00:00 +10:00") }, // 12AM on the day
            new ReadingSession { NumberOfPages = 4, DateOfSession = DateTimeOffset.Parse("2024-07-11 23:59:59 +10:00") }, // Midnight on the the day
            new ReadingSession { NumberOfPages = 8, DateOfSession = DateTimeOffset.Parse("2024-07-12 00:00:00 +10:00") }, // 12 AM day after
            ];

        var mockContext = GetMockContext(existingEntries);
        var service = new ReadingSessionService(_logger, mockContext, timeProvider);

        // Act
        var numberOfPagesRead = await service.GetNumberOfPagesRead(queryDate);

        // Assert
        numberOfPagesRead.ShouldBe(6);
    }

    [Fact]
    public async Task GetNumberOfPagesRead_UseTimeZoneOfTimeProviderLocalNow()
    {
        List<ReadingSession> existingEntries = [
            new ReadingSession { NumberOfPages = 1, DateOfSession = DateTimeOffset.Parse("2024-07-10 13:00:00 +00:00") }, // UTC time (11 pm day before)
            new ReadingSession { NumberOfPages = 2, DateOfSession = DateTimeOffset.Parse("2024-07-10 22:20:00 +00:00") }, // On the day at 8:20AM
            new ReadingSession { NumberOfPages = 4, DateOfSession = DateTimeOffset.Parse("2024-07-11 03:50:00 +00:00") }, // On the day at 1:50PM
            new ReadingSession { NumberOfPages = 8, DateOfSession = DateTimeOffset.Parse("2024-07-11 08:36:00 +00:00") }, // On the day at 6:36PM
            new ReadingSession { NumberOfPages = 16, DateOfSession = DateTimeOffset.Parse("2024-07-11 15:00:00 +00:00") }, // The day after at 1AM
            ];

        var mockContext = GetMockContext(existingEntries);
        var service = new ReadingSessionService(_logger, mockContext, TimeProvider.System);

        // Act
        var utcTime = DateTimeOffset.Parse("2024-07-11 01:00:00 +00:00");
        var numberOfPagesReadUtc = await service.GetNumberOfPagesRead(utcTime);

        var localTime = DateTimeOffset.Parse("2024-07-11 01:00:00 +10:00");
        var numberOfPagesReadLocal = await service.GetNumberOfPagesRead(localTime);

        // Assert
        numberOfPagesReadUtc.ShouldBe(28);
        numberOfPagesReadLocal.ShouldBe(14);
    }

    [Fact]
    public async Task GetNumberOfPages_NoEntries_ShouldReturn0()
    {
        // Arrange
        var dateQuery = DateTimeOffset.Parse("2024-07-12 1:00:00 +10:00");
        var timeProvider = new FakeTimeProvider(dateQuery);

        List<ReadingSession> existingEntries = [
            new ReadingSession { NumberOfPages = 1, DateOfSession = DateTimeOffset.Parse("2024-07-10 23:00:00 +10:00") }, // Yesterday
            new ReadingSession { NumberOfPages = 2, DateOfSession = DateTimeOffset.Parse("2024-07-11 8:20:00 +10:00") }, // Today
            new ReadingSession { NumberOfPages = 4, DateOfSession = DateTimeOffset.Parse("2024-07-11 13:50:00 +10:00") }, // Today
            new ReadingSession { NumberOfPages = 8, DateOfSession = DateTimeOffset.Parse("2024-07-11 18:36:00 +10:00") }, // Today
            ];

        var mockContext = GetMockContext(existingEntries);
        var service = new ReadingSessionService(_logger, mockContext, timeProvider);

        // Act
        var numberOfPagesRead = await service.GetNumberOfPagesRead(dateQuery);

        // Assert
        numberOfPagesRead.ShouldBe(0);
    }

    [Fact]
    public async Task RecordFinishedAt_FirstSession_ShouldUseDefaultStartingPage()
    {
        // Arrange
        List<ReadingSession> existingEntries = []; // No previous entries
        var mockContext = GetMockContext(existingEntries);
        var service = new ReadingSessionService(_logger, mockContext, TimeProvider.System);

        // Act
        var readingSession = await service.RecordFinishedAt(5);

        // Assert
        readingSession.NumberOfPages.ShouldBe(4); // i.e. 5 (finished at) - 1 (default page start)
        readingSession.PageFinishedOn.ShouldBe(5);
    }

    [Fact]
    public async Task RecordFinishedAt_PreviousSession_ShouldCalculatePages()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.Parse("2024-07-12 9:00:00 +10:00"));
        List<ReadingSession> existingEntries = [new ReadingSession { ID = 1, NumberOfPages = 5, PageFinishedOn = 6, DateOfSession = timeProvider.GetLocalNow().AddDays(-1) }];
        var mockContext = GetMockContext(existingEntries);
        var service = new ReadingSessionService(_logger, mockContext, TimeProvider.System);

        // Act
        var readingSession = await service.RecordFinishedAt(15);

        // Assert
        readingSession.NumberOfPages.ShouldBe(9);
        readingSession.PageFinishedOn.ShouldBe(15);
    }

    [Fact]
    public async Task RecordFinishedAt_MultiplePreviousSessions_ShouldGetPreviousSessionByDate()
    {
        List<ReadingSession> existingEntries = [
            new ReadingSession { NumberOfPages = 1, PageFinishedOn = 2, DateOfSession = DateTimeOffset.Parse("2024-07-10 23:00:00 +10:00") }, // First
            new ReadingSession { NumberOfPages = 8, PageFinishedOn = 12, DateOfSession = DateTimeOffset.Parse("2024-07-11 18:36:00 +10:00") }, // This is the latest session
            new ReadingSession { NumberOfPages = 2, PageFinishedOn = 4, DateOfSession = DateTimeOffset.Parse("2024-07-11 8:20:00 +10:00") }, // Second
        ];
        var timeProvider = new FakeTimeProvider(DateTimeOffset.Parse("2024-07-12 9:00:00 +10:00"));
        var mockContext = GetMockContext(existingEntries);
        var service = new ReadingSessionService(_logger, mockContext, timeProvider);

        // Act
        var readingSession = await service.RecordFinishedAt(20);

        // Assert
        readingSession.PageFinishedOn.ShouldBe(20);
        readingSession.NumberOfPages.ShouldBe(8);
    }

    [Fact]
    public async Task RecordFinishedAt_SubsequentRecords()
    {
        // Arrange
        List<ReadingSession> existingEntries = []; // No previous entries
        var mockContext = GetMockContext(existingEntries);
        var service = new ReadingSessionService(_logger, mockContext, TimeProvider.System);

        // Act
        var readingSession1 = await service.RecordFinishedAt(11);
        var readingSession2 = await service.RecordFinishedAt(15);

        // Assert
        readingSession1.NumberOfPages.ShouldBe(10);
        readingSession1.PageFinishedOn.ShouldBe(11);
        readingSession2.NumberOfPages.ShouldBe(4);
        readingSession2.PageFinishedOn.ShouldBe(15);
    }

    [Fact]
    public async Task RecordFinishedAt_SavesToDatabase()
    {
        // Arrange
        var now = DateTimeOffset.Parse("2024-07-11 10:30:00+10:00");
        var timeProvider = new FakeTimeProvider(now);

        List<ReadingSession> existingEntries = [ new ReadingSession { ID = 1, NumberOfPages = 5, PageFinishedOn = 6, DateOfSession = timeProvider.GetLocalNow().AddDays(-1) }];
        var mockSet = existingEntries.AsQueryable().BuildMockDbSet();
        var mockContext = Substitute.For<IPageTrackerDbContext>();

        mockContext.ReadingSessions.Returns(mockSet);
        var service = new ReadingSessionService(_logger, mockContext, timeProvider);

        // Act
        var readingSession = await service.RecordFinishedAt(14);
        _output.WriteLine(readingSession.Serialize());

        // Assert
        readingSession.NumberOfPages.ShouldBe(7);
        readingSession.PageFinishedOn.ShouldBe(14);
        readingSession.DateOfSession.ShouldBe(now);

        mockSet.Received(1).Add(Arg.Is<ReadingSession>(r => r.NumberOfPages == 7 && r.PageFinishedOn == 14 && r.DateOfSession == now && r.ID == 0));
        await mockContext.Received(1).SaveChangesAsync();
    }
}
