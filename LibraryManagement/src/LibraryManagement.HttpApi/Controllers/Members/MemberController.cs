using Asp.Versioning;
using LibraryManagement.Application.Contracts.DTOs.Members;
using LibraryManagement.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace LibraryManagement.HttpApi.Controllers.Members;

[RemoteService(Name = "LibraryManagement")]
[Area("libraryManagement")]
[ControllerName("Member")]
[Route("api/library-management/members")]
public class MemberController : AbpController
{
    private readonly IMemberAppService _memberAppService;

    public MemberController(IMemberAppService memberAppService)
    {
        _memberAppService = memberAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<MemberDto> GetAsync(Guid id)
    {
        return _memberAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<MemberDto>> GetListAsync(GetMemberListInput input)
    {
        return _memberAppService.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<MemberDto> CreateAsync(CreateUpdateMemberDto input)
    {
        return _memberAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<MemberDto> UpdateAsync(Guid id, CreateUpdateMemberDto input)
    {
        return _memberAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _memberAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("expired")]
    public virtual Task<List<MemberDto>> GetExpiredMembersAsync()
    {
        return _memberAppService.GetExpiredMembersAsync();
    }

    [HttpGet]
    [Route("active")]
    public virtual Task<List<MemberDto>> GetActiveMembersAsync()
    {
        return _memberAppService.GetActiveMembersAsync();
    }

    [HttpPost]
    [Route("{id}/renew")]
    public virtual Task<MemberDto> RenewMembershipAsync(Guid id)
    {
        return _memberAppService.RenewMembershipAsync(id);
    }

    [HttpPost]
    [Route("{id}/extend")]
    public virtual Task<MemberDto> ExtendMembershipAsync(Guid id, [FromQuery] int months)
    {
        return _memberAppService.ExtendMembershipAsync(id, months);
    }

    [HttpGet]
    [Route("by-membership-number/{membershipNumber}")]
    public virtual Task<MemberDto> GetByMembershipNumberAsync(string membershipNumber)
    {
        return _memberAppService.GetByMembershipNumberAsync(membershipNumber);
    }

    [HttpGet]
    [Route("by-email/{email}")]
    public virtual Task<MemberDto> GetByEmailAsync(string email)
    {
        return _memberAppService.GetByEmailAsync(email);
    }
}
