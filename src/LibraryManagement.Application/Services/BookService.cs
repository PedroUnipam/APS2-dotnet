using LibraryManagement.Application.DTOs;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;

namespace LibraryManagement.Application.Services
{
    public interface IBookService
    {
        Task<BookDto> CreateAsync(CreateBookRequest request);
        Task<IEnumerable<BookDto>> GetAllAsync();
        Task<BookDto?> GetByIdAsync(Guid id);
        Task<BookDto?> GetByIsbnAsync(string isbn);
        Task<IEnumerable<BookDto>> GetByAuthorAsync(string author);
        Task<BookDto> UpdateAsync(Guid id, UpdateBookRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<BookDto> UpdateStockAsync(Guid id, int totalCopies);
    }

    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookDto> CreateAsync(CreateBookRequest request)
        {
            if (await _bookRepository.ExistsAsync(request.Isbn))
                throw new InvalidOperationException($"A book with ISBN '{request.Isbn}' already exists");

            var book = new Book(request.Title, request.Author, request.Isbn, request.PublicationYear, request.TotalCopies);
            await _bookRepository.AddAsync(book);

            return MapToDto(book);
        }

        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return books.Select(MapToDto);
        }

        public async Task<BookDto?> GetByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return book != null ? MapToDto(book) : null;
        }

        public async Task<BookDto?> GetByIsbnAsync(string isbn)
        {
            var book = await _bookRepository.GetByIsbnAsync(isbn);
            return book != null ? MapToDto(book) : null;
        }

        public async Task<IEnumerable<BookDto>> GetByAuthorAsync(string author)
        {
            var books = await _bookRepository.GetByAuthorAsync(author);
            return books.Select(MapToDto);
        }

        public async Task<BookDto> UpdateAsync(Guid id, UpdateBookRequest request)
        {
            var book = await _bookRepository.GetByIdAsync(id) 
                ?? throw new KeyNotFoundException($"Book with ID '{id}' not found");

            book.UpdateDetails(
                request.Title ?? book.Title,
                request.Author ?? book.Author,
                request.PublicationYear ?? book.PublicationYear
            );

            _bookRepository.Update(book);
            return MapToDto(book);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) return false;

            _bookRepository.Remove(book);
            return true;
        }

        public async Task<BookDto> UpdateStockAsync(Guid id, int totalCopies)
        {
            var book = await _bookRepository.GetByIdAsync(id) 
                ?? throw new KeyNotFoundException($"Book with ID '{id}' not found");

            book.UpdateStock(totalCopies);
            _bookRepository.Update(book);

            return MapToDto(book);
        }

        private static BookDto MapToDto(Book book) => new()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Isbn = book.Isbn,
            PublicationYear = book.PublicationYear,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies,
            CreatedAt = book.CreatedAt
        };
    }
}