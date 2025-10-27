using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans()
        {
            var loans = await _loanService.GetAllAsync();
            return Ok(loans);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetActiveLoans()
        {
            var loans = await _loanService.GetActiveLoansAsync();
            return Ok(loans);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetOverdueLoans()
        {
            var loans = await _loanService.GetOverdueLoansAsync();
            return Ok(loans);
        }

        [HttpGet("member/{memberId:guid}")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoansByMember(Guid memberId)
        {
            var loans = await _loanService.GetLoansByMemberIdAsync(memberId);
            return Ok(loans);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<LoanDto>> GetLoan(Guid id)
        {
            var loan = await _loanService.GetByIdAsync(id);
            if (loan == null)
                return NotFound();

            return Ok(loan);
        }

        [HttpPost]
        public async Task<ActionResult<LoanDto>> CreateLoan(CreateLoanRequest request)
        {
            try
            {
                var loan = await _loanService.CreateAsync(request);
                return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id:guid}/return")]
        public async Task<ActionResult<LoanDto>> ReturnBook(Guid id, ReturnLoanRequest request)
        {
            try
            {
                var loan = await _loanService.ReturnBookAsync(id, request);
                return Ok(loan);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}