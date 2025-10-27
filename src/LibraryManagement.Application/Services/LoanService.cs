using LibraryManagement.Application.DTOs;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;

namespace LibraryManagement.Application.Services
{
    public interface ILoanService
    {
        Task<LoanDto> CreateAsync(CreateLoanRequest request);
        Task<IEnumerable<LoanDto>> GetAllAsync();
        Task<IEnumerable<LoanDto>> GetActiveLoansAsync();
        Task<IEnumerable<LoanDto>> GetLoansByMemberIdAsync(Guid memberId);
        Task<IEnumerable<LoanDto>> GetOverdueLoansAsync();
        Task<LoanDto?> GetByIdAsync(Guid id);
        Task<LoanDto> ReturnBookAsync(Guid id, ReturnLoanRequest request);
    }

    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;

        public LoanService(ILoanRepository loanRepository, IBookRepository bookRepository, IMemberRepository memberRepository)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
        }

        public async Task<LoanDto> CreateAsync(CreateLoanRequest request)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId) 
                ?? throw new KeyNotFoundException($"Book with ID '{request.BookId}' not found");

            var member = await _memberRepository.GetByIdAsync(request.MemberId) 
                ?? throw new KeyNotFoundException($"Member with ID '{request.MemberId}' not found");

            if (!member.IsActive)
                throw new InvalidOperationException("Cannot loan books to inactive members");

            if (book.AvailableCopies <= 0)
                throw new InvalidOperationException("No available copies of this book");

            var loan = new Loan(request.BookId, request.MemberId, DateTime.UtcNow, request.LoanDurationDays);
            book.BorrowCopy();

            await _loanRepository.AddAsync(loan);
            _bookRepository.Update(book);

            return await MapToDto(loan);
        }

        public async Task<IEnumerable<LoanDto>> GetAllAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            var loanDtos = new List<LoanDto>();

            foreach (var loan in loans)
            {
                loanDtos.Add(await MapToDto(loan));
            }

            return loanDtos;
        }

        public async Task<IEnumerable<LoanDto>> GetActiveLoansAsync()
        {
            var loans = await _loanRepository.GetActiveLoansAsync();
            var loanDtos = new List<LoanDto>();

            foreach (var loan in loans)
            {
                loanDtos.Add(await MapToDto(loan));
            }

            return loanDtos;
        }

        public async Task<IEnumerable<LoanDto>> GetLoansByMemberIdAsync(Guid memberId)
        {
            var loans = await _loanRepository.GetLoansByMemberIdAsync(memberId);
            var loanDtos = new List<LoanDto>();

            foreach (var loan in loans)
            {
                loanDtos.Add(await MapToDto(loan));
            }

            return loanDtos;
        }

        public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
        {
            var loans = await _loanRepository.GetOverdueLoansAsync();
            var loanDtos = new List<LoanDto>();

            foreach (var loan in loans)
            {
                loanDtos.Add(await MapToDto(loan));
            }

            return loanDtos;
        }

        public async Task<LoanDto?> GetByIdAsync(Guid id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            return loan != null ? await MapToDto(loan) : null;
        }

        public async Task<LoanDto> ReturnBookAsync(Guid id, ReturnLoanRequest request)
        {
            var loan = await _loanRepository.GetByIdAsync(id) 
                ?? throw new KeyNotFoundException($"Loan with ID '{id}' not found");

            if (loan.IsReturned)
                throw new InvalidOperationException("Book already returned");

            var book = await _bookRepository.GetByIdAsync(loan.BookId) 
                ?? throw new KeyNotFoundException($"Book with ID '{loan.BookId}' not found");

            loan.ReturnBook(request.ReturnDate);
            book.ReturnCopy();

            _loanRepository.Update(loan);
            _bookRepository.Update(book);

            return await MapToDto(loan);
        }

        private async Task<LoanDto> MapToDto(Loan loan)
        {
            var book = await _bookRepository.GetByIdAsync(loan.BookId);
            var member = await _memberRepository.GetByIdAsync(loan.MemberId);

            return new LoanDto
            {
                Id = loan.Id,
                BookId = loan.BookId,
                MemberId = loan.MemberId,
                BookTitle = book?.Title ?? "Unknown",
                MemberName = member != null ? $"{member.FirstName} {member.LastName}" : "Unknown",
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                IsReturned = loan.IsReturned,
                IsOverdue = loan.IsOverdue(DateTime.UtcNow),
                OverdueDays = loan.IsOverdue(DateTime.UtcNow) ? loan.CalculateOverdueDays(DateTime.UtcNow) : null
            };
        }
    }
}