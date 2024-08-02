using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using PageTracker.Application.Books;
using PageTracker.Application.Tests.Common;
using PageTracker.Common.Exceptions;
using PageTracker.Common.Extensions;
using PageTracker.Domain.Models;
using PageTracker.Infrastructure.Persistence;
using PageTracker.Infrastructure.Persistence.Migrations;

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

    [Fact]
    public async Task GetBook_BookDoesntExist_ShouldReturnNull()
    {
        // Arrange
        var books = new List<Book> { new Book { ID = 1, Author = "Test", EndingPage = 100, StartingPage = 1, Title = "Testing", ReadingSessions = [] } };
        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        Book? actualBook = await bookService.GetBook(2);

        // Assert
        actualBook.ShouldBeNull();
    }

    [Fact]
    public async Task GetBook_GetsCorrectBook()
    {
        // Arrange
        var books = new List<Book> 
        { 
            new Book { ID = 1, Author = "Test", EndingPage = 100, StartingPage = 1, Title = "Testing", ReadingSessions = [] },
            new Book { ID = 2, Author = "綾辻 行人", EndingPage = 378, StartingPage = 9, Title = "十角館の殺人", ReadingSessions = [] },
            new Book { ID = 3, Author = "Naomi Novak", EndingPage = 250, StartingPage = 1, Title = "Spinning Silver", ReadingSessions = [] },
        };
        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        Book? actualBook = await bookService.GetBook(2);

        // Assert
        actualBook.ShouldBe(books.First(x => x.ID == 2));
    }

    [Fact]
    public async Task GetBooks_OrdersByAuthor_ThenTitle_JapaneseBooks()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book { ID = 1, Author = "桐野 夏生", EndingPage = 378, StartingPage = 9, Title = "OUT 下", ReadingSessions = [] },
            new Book { ID = 2, Author = "桐野 夏生", EndingPage = 378, StartingPage = 9, Title = "OUT 上", ReadingSessions = [] },
            new Book { ID = 3, Author = "Test", EndingPage = 100, StartingPage = 1, Title = "Aardvarks", ReadingSessions = [] },
            new Book { ID = 4, Author = "綾辻 行人", EndingPage = 378, StartingPage = 9, Title = "水車館の殺人", ReadingSessions = [] }, 
            new Book { ID = 5, Author = "綾辻 行人", EndingPage = 378, StartingPage = 9, Title = "十角館の殺人", ReadingSessions = [] },
            new Book { ID = 6, Author = "Naomi Novak", EndingPage = 250, StartingPage = 1, Title = "Uprooted", ReadingSessions = [] },
            new Book { ID = 7, Author = "Naomi Novak", EndingPage = 250, StartingPage = 1, Title = "Spinning Silver", ReadingSessions = [] },
            
        };
        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var orderedBooks = await bookService.GetBooks();

        // Assert
        orderedBooks.ForEach(x => _output.WriteLine(x.Serialize()));

        // English Authors will appear first
        orderedBooks[0].Author.ShouldBe("Naomi Novak"); // TODO: Consider splitting first name / last name, or a new field for ordering purposes (might solve the Japanese author sorting problem too)
        orderedBooks[0].Title.ShouldBe("Spinning Silver");

        orderedBooks[1].Author.ShouldBe("Naomi Novak");
        orderedBooks[2].Author.ShouldBe("Test");

        // Japanese books will be in Kanji order for now, but at least will be grouped - This test might change
        orderedBooks[3].Author.ShouldBe("桐野 夏生");
        orderedBooks[3].Title.ShouldBe("OUT 上");

        orderedBooks[4].Author.ShouldBe("桐野 夏生");

        orderedBooks[5].Author.ShouldBe("綾辻 行人");
        orderedBooks[5].Title.ShouldBe("十角館の殺人");

        orderedBooks[6].Author.ShouldBe("綾辻 行人");
    }

    [Fact]
    public async Task CreateBook_ShouldIgnoreProvidedID()
    {
        // Arrange
        var mockSet = new List<Book>().AsQueryable().BuildMockDbSet();
        var mockContext = Substitute.For<IPageTrackerDbContext>();
        mockContext.Books.Returns(mockSet);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var newBook = new Book { ID = 3, Author = "Test", EndingPage = 100, StartingPage = 1, Title = "Testing", ReadingSessions = [] };
        var createdBook = await bookService.CreateBook(newBook);

        // Assert
        mockSet.Received(1).Add(Arg.Is<Book>(b => b.ID == 0));
    }

    [Fact]
    public async Task CreateBook_ShouldIgnoreProvidedReadingSessions()
    {
        // Arrange
        var mockSet = new List<Book>().AsQueryable().BuildMockDbSet();
        var mockContext = Substitute.For<IPageTrackerDbContext>();
        mockContext.Books.Returns(mockSet);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var newBook = new Book
        {
            ID = 0,
            Author = "Test",
            EndingPage = 100,
            StartingPage = 1,
            Title = "Testing",
            ReadingSessions = new List<ReadingSession> 
            { 
                new ReadingSession { DateOfSession = DateTimeOffset.Now.AddHours(-1), NumberOfPages = 10, PageFinishedOn = 11 } 
            }
        };
        var createdBook = await bookService.CreateBook(newBook);

        // Assert
        mockSet.Received(1).Add(Arg.Is<Book>(b => !b.ReadingSessions.Any()));
    }

    [Fact]
    public async Task CreateBook_ShouldSaveAllFields()
    {
        // Arrange
        var mockSet = new List<Book>().AsQueryable().BuildMockDbSet();
        var mockContext = Substitute.For<IPageTrackerDbContext>();
        mockContext.Books.Returns(mockSet);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var newBook = new Book
        {
            ID = 0,
            Author = "Test",
            EndingPage = 100,
            StartingPage = 1,
            Title = "Testing",
            ReadingSessions = []
        };
        var createdBook = await bookService.CreateBook(newBook);

        // Assert
        mockSet.Received(1).Add(Arg.Is<Book>(b => b.Author == "Test" && b.EndingPage == 100 && b.StartingPage == 1 && b.Title == "Testing"));
        await mockContext.Received(1).SaveChangesAsync();
    }
}
