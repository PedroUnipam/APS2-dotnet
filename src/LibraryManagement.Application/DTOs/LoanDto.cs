using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.DTOs
{
    public class LoanDto
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }
        public string BookTitle { get; set; }
        public string MemberName { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public bool IsOverdue { get; set; }
        public int? OverdueDays { get; set; }
    }

    public class CreateLoanRequest
    {
        [Required(ErrorMessage = "Book ID is required")]
        public Guid BookId { get; set; }

        [Required(ErrorMessage = "Member ID is required")]
        public Guid MemberId { get; set; }

        [Range(1, 90, ErrorMessage = "Loan duration must be between 1 and 90 days")]
        public int LoanDurationDays { get; set; } = 14;
    }

    public class ReturnLoanRequest
    {
        [Required(ErrorMessage = "Return date is required")]
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    }
}