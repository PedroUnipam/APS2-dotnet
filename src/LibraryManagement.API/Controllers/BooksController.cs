using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var books = await _bookService.GetAllAsync();
            return Ok(books);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BookDto>> GetBook(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<BookDto>> GetBookByIsbn(string isbn)
        {
            var book = await _bookService.GetByIsbnAsync(isbn);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpGet("author/{author}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByAuthor(string author)
        {
            var books = await _bookService.GetByAuthorAsync(author);
            return Ok(books);
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook(CreateBookRequest request)
        {
            try
            {
                var book = await _bookService.CreateAsync(request);
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<BookDto>> UpdateBook(Guid id, UpdateBookRequest request)
        {
            try
            {
                var book = await _bookService.UpdateAsync(id, request);
                return Ok(book);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPatch("{id:guid}/stock")]
        public async Task<ActionResult<BookDto>> UpdateStock(Guid id, [FromBody] int totalCopies)
        {
            try
            {
                var book = await _bookService.UpdateStockAsync(id, totalCopies);
                return Ok(book);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteBook(Guid id)
        {
            var deleted = await _bookService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}