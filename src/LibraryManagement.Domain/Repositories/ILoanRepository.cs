using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan?> GetByIdAsync(Guid id);
        Task<IEnumerable<Loan>> GetAllAsync();
        Task<IEnumerable<Loan>> GetActiveLoansAsync();
        Task<IEnumerable<Loan>> GetLoansByMemberIdAsync(Guid memberId);
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();
        Task AddAsync(Loan loan);
        void Update(Loan loan);
    }
}