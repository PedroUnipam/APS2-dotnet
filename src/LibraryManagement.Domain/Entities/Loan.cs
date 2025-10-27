using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities
{
    public class Loan : BaseEntity
    {
        public Guid BookId { get; private set; }
        public Guid MemberId { get; private set; }
        public DateTime LoanDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime? ReturnDate { get; private set; }
        public bool IsReturned => ReturnDate.HasValue;

        // Navigation properties
        public Book Book { get; private set; }
        public Member Member { get; private set; }

        // Construtor privado para EF
        private Loan() { }

        public Loan(Guid bookId, Guid memberId, DateTime loanDate, int loanDurationDays = 14)
        {
            if (bookId == Guid.Empty)
                throw new ArgumentException("Book ID cannot be empty", nameof(bookId));
            
            if (memberId == Guid.Empty)
                throw new ArgumentException("Member ID cannot be empty", nameof(memberId));
            
            if (loanDurationDays <= 0)
                throw new ArgumentException("Loan duration must be positive", nameof(loanDurationDays));

            BookId = bookId;
            MemberId = memberId;
            LoanDate = loanDate;
            DueDate = loanDate.AddDays(loanDurationDays);
        }

        public void ReturnBook(DateTime returnDate)
        {
            if (IsReturned)
                throw new InvalidOperationException("Book already returned");
            
            if (returnDate < LoanDate)
                throw new ArgumentException("Return date cannot be before loan date");
            
            ReturnDate = returnDate;
        }

        public bool IsOverdue(DateTime currentDate) => 
            !IsReturned && currentDate > DueDate;

        public int CalculateOverdueDays(DateTime currentDate) => 
            IsOverdue(currentDate) ? (currentDate - DueDate).Days : 0;
    }
}