using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Repositories
{
    public interface IMemberRepository
    {
        Task<Member?> GetByIdAsync(Guid id);
        Task<IEnumerable<Member>> GetAllAsync();
        Task<IEnumerable<Member>> GetActiveMembersAsync();
        Task<Member?> GetByEmailAsync(string email);
        Task AddAsync(Member member);
        void Update(Member member);
        Task<bool> ExistsAsync(string email);
    }
}