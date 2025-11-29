using AutoMapper.Internal.Mappers;
using LibraryManagement.Application.Contracts.DTOs.Members;
using LibraryManagement.Application.Contracts.Permissions;
using LibraryManagement.Application.Contracts.Services;
using LibraryManagement.Domain.DomainServices;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using LibraryManagement.Domain.Shared.Constants;
using LibraryManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace LibraryManagement.Application.Services.Members;

public class MemberAppService : CrudAppService<
    Member,
    MemberDto,
    Guid,
    GetMemberListInput,
    CreateUpdateMemberDto,
    CreateUpdateMemberDto>, IMemberAppService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly MemberManager _memberManager;

    public MemberAppService(
        IMemberRepository repository,
        ILoanRepository loanRepository,
        MemberManager memberManager)
        : base(repository)
    {
        _memberRepository = repository;
        _loanRepository = loanRepository;
        _memberManager = memberManager;
    }

    protected override async Task<IQueryable<Member>> CreateFilteredQueryAsync(GetMemberListInput input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        query = query
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                x => x.FirstName.Contains(input.Filter!) ||
                     x.LastName.Contains(input.Filter!) ||
                     x.Email.Contains(input.Filter!) ||
                     x.MembershipNumber.Contains(input.Filter!))
            .WhereIf(input.MembershipType.HasValue,
                x => x.MembershipType == input.MembershipType!.Value)
            .WhereIf(input.IsExpired.HasValue && input.IsExpired.Value,
                x => x.ExpiryDate < DateTime.UtcNow)
            .WhereIf(input.IsExpired.HasValue && !input.IsExpired.Value,
                x => x.ExpiryDate >= DateTime.UtcNow);

        return query;
    }

    public override async Task<MemberDto> GetAsync(Guid id)
    {
        var member = await _memberRepository.GetAsync(id);
        var dto = ObjectMapper.Map<Member, MemberDto>(member);

        // Get active loan count
        dto.ActiveLoanCount = await _loanRepository.GetActiveLoanCountByMemberIdAsync(id);

        return dto;
    }

    public override async Task<MemberDto> CreateAsync(CreateUpdateMemberDto input)
    {
        Address? address = null;
        if (input.Address != null)
        {
            address = ObjectMapper.Map<CreateUpdateAddressDto, Address>(input.Address);
        }

        var member = await _memberManager.CreateAsync(
            input.FirstName,
            input.LastName,
            input.MembershipNumber,
            input.Email,
            input.MembershipType,
            input.PhoneNumber,
            address);

        await _memberRepository.InsertAsync(member);

        return await GetAsync(member.Id);
    }

    public override async Task<MemberDto> UpdateAsync(Guid id, CreateUpdateMemberDto input)
    {
        var member = await _memberRepository.GetAsync(id);

        if (member.MembershipNumber != input.MembershipNumber)
        {
            await _memberManager.ChangeMembershipNumberAsync(member, input.MembershipNumber);
        }

        if (member.Email != input.Email)
        {
            await _memberManager.ChangeEmailAsync(member, input.Email);
        }

        member.SetFirstName(input.FirstName)
              .SetLastName(input.LastName)
              .SetPhoneNumber(input.PhoneNumber)
              .SetMembershipType(input.MembershipType);

        if (input.Address != null)
        {
            var address = ObjectMapper.Map<CreateUpdateAddressDto, Address>(input.Address);
            member.SetAddress(address);
        }
        else
        {
            member.SetAddress(null);
        }

        await _memberRepository.UpdateAsync(member);

        return await GetAsync(member.Id);
    }

    public override async Task DeleteAsync(Guid id)
    {
        var member = await _memberRepository.GetAsync(id);
        await _memberManager.ValidateMemberDeletionAsync(member);
        await base.DeleteAsync(id);
    }

    public async Task<List<MemberDto>> GetExpiredMembersAsync()
    {
        var members = await _memberRepository.GetExpiredMembersAsync();
        return ObjectMapper.Map<List<Member>, List<MemberDto>>(members);
    }

    public async Task<List<MemberDto>> GetActiveMembersAsync()
    {
        var members = await _memberRepository.GetActiveMembersAsync();
        return ObjectMapper.Map<List<Member>, List<MemberDto>>(members);
    }

    public async Task<MemberDto> RenewMembershipAsync(Guid id)
    {
        var member = await _memberRepository.GetAsync(id);
        member.RenewMembership();
        await _memberRepository.UpdateAsync(member);
        return await GetAsync(id);
    }

    public async Task<MemberDto> ExtendMembershipAsync(Guid id, int months)
    {
        var member = await _memberRepository.GetAsync(id);
        member.ExtendMembership(months);
        await _memberRepository.UpdateAsync(member);
        return await GetAsync(id);
    }

    public async Task<MemberDto> GetByMembershipNumberAsync(string membershipNumber)
    {
        var member = await _memberRepository.FindByMembershipNumberAsync(membershipNumber);

        if (member == null)
        {
            throw new Volo.Abp.BusinessException(LibraryManagementErrorCodes.MemberNotFound)
                .WithData("MembershipNumber", membershipNumber);
        }

        return await GetAsync(member.Id);
    }

    public async Task<MemberDto> GetByEmailAsync(string email)
    {
        var member = await _memberRepository.FindByEmailAsync(email);

        if (member == null)
        {
            throw new Volo.Abp.BusinessException(LibraryManagementErrorCodes.MemberNotFound)
                .WithData("Email", email);
        }

        return await GetAsync(member.Id);
    }
}
