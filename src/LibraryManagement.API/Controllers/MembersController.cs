using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers()
        {
            var members = await _memberService.GetAllAsync();
            return Ok(members);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetActiveMembers()
        {
            var members = await _memberService.GetActiveMembersAsync();
            return Ok(members);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MemberDto>> GetMember(Guid id)
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null)
                return NotFound();

            return Ok(member);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<MemberDto>> GetMemberByEmail(string email)
        {
            var member = await _memberService.GetByEmailAsync(email);
            if (member == null)
                return NotFound();

            return Ok(member);
        }

        [HttpPost]
        public async Task<ActionResult<MemberDto>> CreateMember(CreateMemberRequest request)
        {
            try
            {
                var member = await _memberService.CreateAsync(request);
                return CreatedAtAction(nameof(GetMember), new { id = member.Id }, member);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MemberDto>> UpdateMember(Guid id, UpdateMemberRequest request)
        {
            try
            {
                var member = await _memberService.UpdateAsync(id, request);
                return Ok(member);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPatch("{id:guid}/deactivate")]
        public async Task<ActionResult> DeactivateMember(Guid id)
        {
            var deactivated = await _memberService.DeactivateAsync(id);
            if (!deactivated)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id:guid}/activate")]
        public async Task<ActionResult> ActivateMember(Guid id)
        {
            var activated = await _memberService.ActivateAsync(id);
            if (!activated)
                return NotFound();

            return NoContent();
        }
    }
}