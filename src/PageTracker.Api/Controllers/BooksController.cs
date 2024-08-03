using Asp.Versioning;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PageTracker.Application.Books;
using PageTracker.Common.Exceptions;
using PageTracker.Domain.Models;

namespace PageTracker.Api.Controllers;

/// <summary>
/// Endpoints to get, create, update and delete books.
/// </summary>
[ApiController]
[Route("books")]
[ApiVersion("1.0")]
public class BooksController(ILogger<BooksController> logger, IBookService bookService) : ControllerBase
{
    /// <summary>
    /// Gets a book
    /// </summary>
    /// <param name="id">The ID of the book to get</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns requested book</response>
    /// <response code="404">Book was not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType<Book>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> Get([FromRoute]int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var book = await bookService.GetBook(id, cancellationToken);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An uxpected error occured when trying to get book {BookID}", id);
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "An unxpected error occurred when trying to get this book.");
        }
    }

    /// <summary>
    /// Gets all books
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns all books</response>
    [HttpGet]
    [ProducesResponseType<List<Book>>(StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        try
        {
            var books = await bookService.GetBooks(cancellationToken);
            return Ok(books);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An uxpected error occured when trying to get all books");
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "An unxpected error occurred when trying to get all books.");
        }
    }

    /// <summary>
    /// Creates a new book
    /// </summary>
    /// <param name="book">Details about the book to create. ID and Reading Session values will be ignored.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="201">Sucessfully created a book</response>
    /// <response code="400">There were validation errors</response>
    [HttpPost]
    [ProducesResponseType<Book>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> Create(Book book, CancellationToken cancellationToken = default)
    {
        try
        {
            var newBook = await bookService.CreateBook(book, cancellationToken);
            return Created(new Uri(Request.GetEncodedUrl() + "/" + newBook.ID), newBook);
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Validation error occurred");
            ModelState.AddModelError(ex.ParamName ?? nameof(book), ex.Message);
            return BadRequest(ModelState);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An uxpected error occured when trying to create the book");
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "An unxpected error occurred when trying to create this book.");
        }
    }

    /// <summary>
    /// Updates the given book. Starting page can't be edited if the book has already been started.
    /// </summary>
    /// <param name="id">ID of the book to update</param>
    /// <param name="book">Book with its updated details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Book was successfully updated</response>
    /// <response code="400">There were validation errors</response>
    /// <response code="422">Provided update was not allowed</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> Update([FromRoute]int id, Book book, CancellationToken cancellationToken = default)
    {
        try
        {
            var updatedBook = await bookService.UpdateBook(id, book, cancellationToken);
            return Ok(updatedBook);
        }
        catch (RecordNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Validation error occurred");
            ModelState.AddModelError(ex.ParamName ?? nameof(book), ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (ApplicationException ex)
        {
            return Problem(detail: ex.Message, title: ex.Message, statusCode: StatusCodes.Status422UnprocessableEntity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An uxpected error occured when trying to updated book {BookID}", id);
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "An unxpected error occurred when trying to updated this book.");
        }
    }

    /// <summary>
    /// Deletes the book if it hasn't already been started, i.e. there are no associated reading sessions
    /// </summary>
    /// <param name="id">The ID of the book to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Book was successfully deleted</response>
    /// <response code="404">Book was not found</response>
    /// <response code="422">Book had already been started</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> Delete([FromRoute]int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await bookService.DeleteBook(id, cancellationToken);
            return Ok();
        }
        catch (RecordNotFoundException)
        {
            return NotFound();
        }
        catch (ApplicationException ex)
        {
            return Problem(detail: ex.Message, title: ex.Message, statusCode: StatusCodes.Status422UnprocessableEntity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An uxpected error occured when trying to delete book {BookID}", id);
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError, 
                title: "An unxpected error occurred when trying to delete this book.");
        }
    }
}
