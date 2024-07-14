using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using PageTracker.Application.ReadingSessions;
using PageTracker.Infrastructure.Persistence;

namespace PageTracker.Application.Tests.ReadingSessions;

public class ReadingSessionsRegistrationTests
{
    private readonly IServiceProvider _serviceProvider;

    public ReadingSessionsRegistrationTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddSingleton<TimeProvider>(new FakeTimeProvider(DateTimeOffset.Parse("2024-07-14 11:00:00")));
        serviceCollection.AddSingleton(Substitute.For<IPageTrackerDbContext>());
        serviceCollection.AddReadingSessions();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void Register_ReadingSessionsService()
    {
        // Arrange

        // Act
        var readingSessionService = _serviceProvider.GetService<IReadingSessionService>();

        // Assert
        readingSessionService.ShouldNotBeNull();
        readingSessionService.ShouldBeAssignableTo<ReadingSessionService>();
    }
}
