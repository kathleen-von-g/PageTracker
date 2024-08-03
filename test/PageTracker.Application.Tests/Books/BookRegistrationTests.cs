using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using PageTracker.Application.Books;
using PageTracker.Application.ReadingSessions;
using PageTracker.Infrastructure.Persistence;

namespace PageTracker.Application.Tests.Books;
public class BookRegistrationTests
{
    private readonly IServiceProvider _serviceProvider;

    public BookRegistrationTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddSingleton<TimeProvider>(new FakeTimeProvider(DateTimeOffset.Parse("2024-07-14 11:00:00")));
        serviceCollection.AddSingleton(Substitute.For<IPageTrackerDbContext>());
        serviceCollection.AddBooks();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void Register_BookService()
    {
        // Arrange

        // Act
        var bookService = _serviceProvider.GetService<IBookService>();

        // Assert
        bookService.ShouldNotBeNull();
        bookService.ShouldBeAssignableTo<BookService>();
    }
}
