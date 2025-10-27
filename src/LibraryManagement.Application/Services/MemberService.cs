using LibraryManagement.Application.DTOs;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;

namespace LibraryManagement.Application.Services
{
    public interface IMemberService
    {
        Task<MemberDto> CreateAsync(CreateMemberRequest request);
        Task<IEnumerable<MemberDto>> GetAllAsync();
        Task<IEnumerable<MemberDto>> GetActiveMembersAsync();
        Task<MemberDto?> GetByIdAsync(Guid id);
        Task<MemberDto?> GetByEmailAsync(string email);
        Task<MemberDto> UpdateAsync(Guid id, UpdateMemberRequest request);
        Task<bool> DeactivateAsync(Guid id);
        Task<bool> ActivateAsync(Guid id);
    }

    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<MemberDto> CreateAsync(CreateMemberRequest request)
        {
            if (await _memberRepository.ExistsAsync(request.Email))
                throw new InvalidOperationException($"A member with email '{request.Email}' already exists");

            var member = new Member(request.FirstName, request.LastName, request.Email, request.Phone);
            await _memberRepository.AddAsync(member);

            return MapToDto(member);
        }

        public async Task<IEnumerable<MemberDto>> GetAllAsync()
        {
            var members = await _memberRepository.GetAllAsync();
            return members.Select(MapToDto);
        }

        public async Task<IEnumerable<MemberDto>> GetActiveMembersAsync()
        {
            var members = await _memberRepository.GetActiveMembersAsync();
            return members.Select(MapToDto);
        }

        public async Task<MemberDto?> GetByIdAsync(Guid id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            return member != null ? MapToDto(member) : null;
        }

        public async Task<MemberDto?> GetByEmailAsync(string email)
        {
            var member = await _memberRepository.GetByEmailAsync(email);
            return member != null ? MapToDto(member) : null;
        }

        public async Task<MemberDto> UpdateAsync(Guid id, UpdateMemberRequest request)
        {
            var member = await _memberRepository.GetByIdAsync(id) 
                ?? throw new KeyNotFoundException($"Member with ID '{id}' not found");

            member.UpdateDetails(
                request.FirstName ?? member.FirstName,
                request.LastName ?? member.LastName,
                request.Email ?? member.Email,
                request.Phone
            );

            _memberRepository.Update(member);
            return MapToDto(member);
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null) return false;

            member.Deactivate();
            _memberRepository.Update(member);
            return true;
        }

        public async Task<bool> ActivateAsync(Guid id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null) return false;

            member.Activate();
            _memberRepository.Update(member);
            return true;
        }

        private static MemberDto MapToDto(Member member) => new()
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Email = member.Email,
            Phone = member.Phone,
            RegistrationDate = member.RegistrationDate,
            IsActive = member.IsActive
        };
    }
}