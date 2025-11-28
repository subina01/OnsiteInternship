using LibraryManagement.Application.Contracts.DTOs.Members;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace LibraryManagement.Application.Contracts.Services;

public interface IMemberAppService : ICrudAppService<
    MemberDto,
    Guid,
    GetMemberListInput,
    CreateUpdateMemberDto,
    CreateUpdateMemberDto>
{
    Task<List<MemberDto>> GetExpiredMembersAsync();
    Task<List<MemberDto>> GetActiveMembersAsync();
    Task<MemberDto> RenewMembershipAsync(Guid id);
    Task<MemberDto> ExtendMembershipAsync(Guid id, int months);
    Task<MemberDto> GetByMembershipNumberAsync(string membershipNumber);
    Task<MemberDto> GetByEmailAsync(string email);
}
