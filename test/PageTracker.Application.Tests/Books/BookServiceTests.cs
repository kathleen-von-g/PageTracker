using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using PageTracker.Application.Books;
using PageTracker.Application.Tests.Common;
using PageTracker.Common.Exceptions;
using PageTracker.Domain.Models;
using PageTracker.Infrastructure.Persistence;

namespace PageTracker.Application.Tests.Books;

public class BookServiceTests
{
    private readonly ITestOutputHelper _output;
    private readonly ILogger<BookService> _logger;

    public BookServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _logger = new TestOutputLogger<BookService>(output);
    }

    private static IPageTrackerDbContext GetMockContext(List<Book> existingEntries)
    {
        var mockSet = existingEntries.AsQueryable().BuildMockDbSet();
        var mockContext = Substitute.For<IPageTrackerDbContext>();
        mockContext.Books.Returns(mockSet);
        return mockContext;
    }

    [Fact]
    public async Task DeleteBook_ThrowsNotFoundException_IfNoBookExists()
    {
        // Arrange
        var books = new List<Book> { new Book { ID = 1, Author = "Test", EndingPage = 100, StartingPage = 1, Title = "Testing", ReadingSessions = [] } };
        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var actualException = await Record.ExceptionAsync(() => bookService.DeleteBook(2));

        // Assert
        actualException.ShouldNotBeNull();
        actualException.ShouldBeAssignableTo<RecordNotFoundException>();
        actualException.Data["BookID"].ShouldNotBeNull().ShouldBe(2);
    }

    [Fact]
    public async Task DeleteBook_ThrowsApplicationException_IfBookAlreadyStarted()
    {
        // Arrange
        var book = new Book
        {
            ID = 1,
            Author = "Test",
            EndingPage = 100,
            StartingPage = 1,
            Title = "Testing",
            ReadingSessions = new List<ReadingSession> {
                new ReadingSession
                {
                    DateOfSession = DateTimeOffset.Now.AddDays(-1),
                    NumberOfPages =  10,
                    PageFinishedOn = 11,
                    BookID = 1,
                    ID = 1
                }
            }
        };

        var books = new List<Book> { book };

        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var actualException = await Record.ExceptionAsync(() => bookService.DeleteBook(1));

        // Assert
        actualException.ShouldNotBeNull();
        actualException.ShouldBeAssignableTo<PageTracker.Common.Exceptions.ApplicationException>();
        actualException.Data["BookID"].ShouldNotBeNull().ShouldBe(1);
    }

    [Fact]
    public async Task DeleteBook_BookIsDeleted()
    {
        // Arrange
        var books = new List<Book> { new Book { ID = 1, Author = "Test", EndingPage = 100, StartingPage = 1, Title = "Testing", ReadingSessions = [] } };
        var mockSet = books.AsQueryable().BuildMockDbSet();
        var mockContext = Substitute.For<IPageTrackerDbContext>();
        mockContext.Books.Returns(mockSet);
        var bookService = new BookService(_logger, mockContext);

        // Act
        await bookService.DeleteBook(1);

        // Assert
        mockSet.Received(1).Remove(books[0]);
        await mockContext.Received(1).SaveChangesAsync();
    }
}
