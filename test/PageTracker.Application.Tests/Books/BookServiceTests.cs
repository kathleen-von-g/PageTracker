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
    public async Task CreateBook_ThrowsArgumentException_IfEndingPageLessThanStartingPage()
    {
        // Arrange
        var mockSet = new List<Book>().AsQueryable().BuildMockDbSet();
        var mockContext = Substitute.For<IPageTrackerDbContext>();
        mockContext.Books.Returns(mockSet);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var newBook = new Book { ID = 3, Author = "Test", EndingPage = 1, StartingPage = 9, Title = "Testing", ReadingSessions = [] };
        var actualException = await Record.ExceptionAsync(() => bookService.CreateBook(newBook));

        // Assert
        actualException.ShouldNotBeNull();
        actualException.ShouldBeAssignableTo<ArgumentException>();        
    }

    [Fact]
    public async Task CreateBook_EndingPage_CanBeTheSameAsStartingPage()
    {
        // Arrange
        var mockSet = new List<Book>().AsQueryable().BuildMockDbSet();
        var mockContext = Substitute.For<IPageTrackerDbContext>();
        mockContext.Books.Returns(mockSet);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var newBook = new Book { ID = 3, Author = "Test", EndingPage = 9, StartingPage = 9, Title = "One page book", ReadingSessions = [] };
        var createdBook = await bookService.CreateBook(newBook);

        // Assert
        mockSet.Received(1).Add(Arg.Is<Book>(b => b.ID == 0 && b.EndingPage == 9 && b.StartingPage == 9));
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

    [Fact]
    public async Task UpdateBook_ThrowsRecordNotFoundException_IfDoesntExist()
    {
        // Arrange
        var books = new List<Book> { new Book { ID = 1, Author = "Test", EndingPage = 100, StartingPage = 1, Title = "Testing", ReadingSessions = [] } };
        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var actualException = await Record.ExceptionAsync(() => bookService.UpdateBook(2, new Book { Author = "Updated Author", StartingPage = 2, EndingPage = 200, Title = "Updated Title", ReadingSessions = [] }));

        // Assert
        actualException.ShouldNotBeNull();
        actualException.ShouldBeAssignableTo<RecordNotFoundException>();
        actualException.Data["BookID"].ShouldBe(2);
    }

    [Fact]
    public async Task UpdateBack_ThrowsApplicationException_IfEditingStartPage_IfAlreadyStarted()
    {
        // Arrange
        var books = new List<Book> { new Book
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
        }};
        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var actualException = await Record.ExceptionAsync(() => bookService.UpdateBook(1, new Book { Author = "Test", StartingPage = 2, EndingPage = 100, Title = "Testing", ReadingSessions = [] }));

        // Assert
        actualException.ShouldNotBeNull();
        actualException.ShouldBeAssignableTo<PageTracker.Common.Exceptions.ApplicationException>();
        actualException.Data["BookID"].ShouldBe(1);
    }

    [Fact]
    public async Task UpdateBook_ShouldIgnoreProvidedBookID()
    {
        // Arrange
        var book1 = new Book { ID = 1, Author = "Test", EndingPage = 100, StartingPage = 1, Title = "Testing", ReadingSessions = [] };
        var book2 = new Book { ID = 2, Author = "Book 2 Author", EndingPage = 378, StartingPage = 9, Title = "Book 2 Title", ReadingSessions = [] };
        var books = new List<Book> { book1, book2 };
        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        // Update book 1
        var updatedBook = await bookService.UpdateBook(1, new Book { ID = 2, Author = "Updated Author", StartingPage = 2, EndingPage = 200, Title = "Updated Title", ReadingSessions = [] });

        // Assert
        updatedBook.ID.ShouldBe(1);
        book1.Author.ShouldBe("Updated Author");
        book2.Author.ShouldBe("Book 2 Author");
    }

    [Fact]
    public async Task UpdateBook_ShouldIgnoreProvidedReadingSessions()
    {
        // Arrange
        var books = new List<Book> { new Book { ID = 1, Author = "Test", StartingPage = 1, EndingPage = 100, Title = "Testing", ReadingSessions = new List<ReadingSession> { DummyReadingSession(1) } } };
        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var bookDetails = new Book { ID = 1, Author = "Updated Author", StartingPage = 1, EndingPage = 100, Title = "Updated Title", ReadingSessions = new List<ReadingSession> { DummyReadingSession(2) } };
        await bookService.UpdateBook(1, bookDetails);

        // Assert
        books.First().ReadingSessions.First().ID.ShouldBe(1);
        books.First().ReadingSessions.Count.ShouldBe(1);
    }

    [Fact]
    public async Task UpdateBook_ExistingBook_IsUpdated()
    {
        // Arrange
        var book1 = new Book { ID = 1, Author = "Test", EndingPage = 100, StartingPage = 1, Title = "Testing", ReadingSessions = [] };
        var books = new List<Book> { book1 };
        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        // Update book 1
        var updatedBook = await bookService.UpdateBook(1, new Book { ID = 1, Author = "Updated Author", StartingPage = 2, EndingPage = 200, Title = "Updated Title", ReadingSessions = [] });

        // Assert
        updatedBook.ID.ShouldBe(1);
        book1.ID.ShouldBe(1);
        book1.Author.ShouldBe("Updated Author");
        book1.Title.ShouldBe("Updated Title");
        book1.StartingPage.ShouldBe(2);
        book1.EndingPage.ShouldBe(200);
        book1.ReadingSessions.ShouldBeEmpty();
        await mockContext.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateBook_ThrowsArgumentException_IfEndingPageLessThanStartingPage()
    {
        // Arrange
        var books = new List<Book> { new Book { ID = 1, Author = "Test", StartingPage = 10, EndingPage = 100, Title = "Testing", ReadingSessions = [] } };
        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var bookDetails = new Book { ID = 1, Author = "Updated Author", StartingPage = 10, EndingPage = 9, Title = "Updated Title", ReadingSessions = [] };
        var actualException = await Record.ExceptionAsync(() => bookService.UpdateBook(1, bookDetails));

        // Assert
        actualException.ShouldNotBeNull();
        actualException.ShouldBeAssignableTo<ArgumentException>();
    }

    [Fact]
    public async Task UpdateBook_EndingPage_CanBeTheSameAsStartingPage()
    {
        // Arrange
        var books = new List<Book> { new Book { ID = 1, Author = "Test", StartingPage = 10, EndingPage = 100, Title = "Testing", ReadingSessions = [] } };
        var mockContext = GetMockContext(books);
        var bookService = new BookService(_logger, mockContext);

        // Act
        var bookDetails = new Book { ID = 1, Author = "Updated Author", StartingPage = 10, EndingPage = 10, Title = "Updated Title", ReadingSessions = [] };
        await bookService.UpdateBook(1, bookDetails);

        // Assert
        books.First().StartingPage.ShouldBe(10);
        books.First().EndingPage.ShouldBe(10);
    }

    private ReadingSession DummyReadingSession(int id) => new ReadingSession
    {
        ID = id,
        DateOfSession = DateTimeOffset.Now.AddDays(-1),
        NumberOfPages = 10,
        PageFinishedOn = 1
    };
}
