using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PageTracker.Common.Exceptions;
using PageTracker.Common.Extensions;
using PageTracker.Domain.Models;
using PageTracker.Infrastructure.Persistence;

namespace PageTracker.Application.Books;

public interface IBookService
{
    /// <summary>
    /// Gets a book by its ID
    /// </summary>
    /// <param name="id">The unique ID of the book</param>
    /// <returns>A book, or null if the book doesn't exist</returns>
    Task<Book?> GetBook(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all books, ordered by Author, then by Title
    /// </summary>
    /// <returns>A list of books</returns>
    Task<List<Book>> GetBooks(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new book 
    /// </summary>
    /// <param name="newBook">Details about the new book, with ID set to 0</param>
    /// <returns>The created book</returns>
    Task<Book> CreateBook(Book newBook, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the book with the given id with information provided. Does not override reading sessions. 
    /// </summary>
    /// <param name="id">The ID of the book to update</param>
    /// <param name="updatedBook"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException">If the starting page has been edited once the book has already been started</exception>
    /// <exception cref="ArgumentException">If there were validation errors</exception>
    /// <exception cref="RecordNotFoundException">The book to update doesn't exist</exception>
    Task<Book> UpdateBook(int id, Book updatedBook, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete the book with the given id. Books that have already been started cannot be deleted.
    /// </summary>
    /// <param name="id">The ID of the book to delete</param>
    /// <exception cref="ApplicationException">The provided book has already been started</exception>
    /// <exception cref="ArgumentException">If there were validation errors</exception>
    /// <exception cref="RecordNotFoundException">The book to delete doesn't exist</exception>
    Task DeleteBook(int id, CancellationToken cancellationToken = default);
}

internal class BookService(ILogger<BookService> logger, IPageTrackerDbContext context) : IBookService
{
    public Task<Book?> GetBook(int id, CancellationToken cancellationToken = default)
    {
        return context.Books.FirstOrDefaultAsync(x => x.ID == id, cancellationToken);
    }

    public Task<List<Book>> GetBooks(CancellationToken cancellationToken = default)
    {
        return context.Books.OrderBy(x => x.Author).ThenBy(x => x.Title).ToListAsync(cancellationToken);
    }

    public async Task<Book> CreateBook(Book newBook, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new book {Book}", newBook.Serialize());

        // Ending page can't be less than starting page
        if (newBook.EndingPage < newBook.StartingPage)
        {
            throw new ArgumentException("Ending Page can't be earlier than the Starting Page", nameof(Book.EndingPage));
        }

        // Ignore ID
        newBook.ID = 0;

        // Ignore reading sessions
        newBook.ReadingSessions.Clear();

        try
        {
            context.Books.Add(newBook);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Successfully created book {BookID}", newBook.ID);
            return newBook;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred when creating book");
            throw;
        }
    }

    public async Task<Book> UpdateBook(int id, Book updatedBook, CancellationToken cancellationToken = default)
    {
        // Update existing book
        Book? existingBook = await context.Books.Include(x => x.ReadingSessions).FirstOrDefaultAsync(x => x.ID == id, cancellationToken);
        if (existingBook == null)
        {
            var exception = new RecordNotFoundException(typeof(Book), id);
            exception.Data.Add("BookID", id);

            logger.LogError(exception, "Book {BookID} was not found.", id);
            throw exception;
        }

        // Can't update book's starting page if the book has already been started (i.e. has reading sessions)
        if (existingBook.StartingPage != updatedBook.StartingPage && existingBook.ReadingSessions.Any())
        {
            var exception = new ApplicationException($"Cannot update the starting page of \"{existingBook.Title}\" because it's already been started.");
            exception.Data.Add("BookID", id);

            logger.LogError(exception, "Book starting page can't be updated. Book {BookID} has already been started.", id);
            throw exception;
        }

        // Ending page can't be less than starting page
        if (updatedBook.EndingPage < updatedBook.StartingPage)
        {
            throw new ArgumentException("Ending Page can't be earlier than the Starting Page", nameof(Book.EndingPage));
        }

        logger.LogInformation("Updating book {BookID} with {Book}", id, updatedBook);

        try
        {
            existingBook.Author = updatedBook.Author;
            existingBook.Title = updatedBook.Title;
            existingBook.StartingPage = updatedBook.StartingPage;
            existingBook.EndingPage = updatedBook.EndingPage;
            await context.SaveChangesAsync(cancellationToken);

            return existingBook;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred when updating book {BookID}", id);
            throw;
        }
    }

    public async Task DeleteBook(int id, CancellationToken cancellationToken = default)
    {
        Book? existingBook = await context.Books.Include(x => x.ReadingSessions).FirstOrDefaultAsync(x => x.ID == id, cancellationToken);
        if (existingBook == null)
        {
            var exception = new RecordNotFoundException(typeof(Book), id);
            exception.Data.Add("BookID", id);

            logger.LogError(exception, "Book {BookID} was not found.", id);
            throw exception;
        }
        
        // Books can't be deleted if the book has already been started (i.e. has reading sessions)
        if (existingBook.ReadingSessions.Any())
        {
            var exception = new ApplicationException($"Cannot delete \"{existingBook.Title}\" because it's already been started.");
            exception.Data.Add("BookID", id);

            logger.LogError(exception, "Book can't be deleted. Book {BookID} has already been started.", id);
            throw exception;
        }

        logger.LogInformation("Deleting book {BookID}", id);

        try
        {
            context.Books.Remove(existingBook);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred when deleting book {BookID}", id);
            throw;
        }
    }
}
