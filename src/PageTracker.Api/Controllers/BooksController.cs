using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PageTracker.Application.Books;

namespace PageTracker.Api.Controllers;

[ApiController]
[Route("books")]
[ApiVersion("1.0")]
public class BooksController(ILogger<BooksController> logger, IBookService bookService) : ControllerBase
{

}
