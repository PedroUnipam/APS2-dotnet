using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Repositories
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(Guid id);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<IEnumerable<Book>> GetByAuthorAsync(string author);
        Task<Book?> GetByIsbnAsync(string isbn);
        Task AddAsync(Book book);
        void Update(Book book);
        void Remove(Book book);
        Task<bool> ExistsAsync(string isbn);
    }
}