using Microsoft.EntityFrameworkCore;
using PageTracker.Common.Exceptions;
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
    /// <exception cref="InvalidOperationException">The provided book already has an ID</exception>
    Task<Book> CreateBook(Book newBook, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the book with the given id with information provided. Does not override reading sessions. 
    /// </summary>
    /// <param name="id">The ID of the book to update</param>
    /// <param name="updatedBook"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">The provided book's ID doesn't match the given book</exception>
    /// <exception cref="ArgumentException">If the starting page has been edited once the book has already been started</exception>
    /// <exception cref="RecordNotFoundException">The book to update doesn't exist</exception>
    Task<Book> UpdateBook(int id, Book updatedBook, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete the book with the given id. Books that have already been started cannot be deleted.
    /// </summary>
    /// <param name="id">The ID of the book to delete</param>
    /// <exception cref="InvalidOperationException">The provided book has already been started</exception>
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

    public Task<Book> CreateBook(Book newBook, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new book....");
        throw new NotImplementedException();
    }

    public Task<Book> UpdateBook(int id, Book updatedBook, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteBook(int id, CancellationToken cancellationToken = default)
    {
        Book? existingBook = await GetBook(id, cancellationToken);
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
