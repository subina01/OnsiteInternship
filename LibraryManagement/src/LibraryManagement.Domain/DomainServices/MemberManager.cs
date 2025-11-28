using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using LibraryManagement.Domain.Shared.Constants;
using LibraryManagement.Domain.Shared.Enums;
using LibraryManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace LibraryManagement.Domain.DomainServices;

/// <summary>
/// Domain Service for managing Member business logic
/// </summary>
public class MemberManager : DomainService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILoanRepository _loanRepository;

    public MemberManager(
        IMemberRepository memberRepository,
        ILoanRepository loanRepository)
    {
        _memberRepository = memberRepository;
        _loanRepository = loanRepository;
    }

    public async Task<Member> CreateAsync(
        string firstName,
        string lastName,
        string membershipNumber,
        string email,
        MembershipType membershipType,
        string? phoneNumber = null,
        Address? address = null)
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName));
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName));
        Check.NotNullOrWhiteSpace(membershipNumber, nameof(membershipNumber));
        Check.NotNullOrWhiteSpace(email, nameof(email));

        // Check if membership number already exists
        if (await _memberRepository.IsMembershipNumberExistsAsync(membershipNumber))
        {
            throw new BusinessException(LibraryManagementErrorCodes.DuplicateMembershipNumber)
                .WithData("MembershipNumber", membershipNumber);
        }

        // Check if email already exists
        if (await _memberRepository.IsEmailExistsAsync(email))
        {
            throw new BusinessException(LibraryManagementErrorCodes.MemberAlreadyExists)
                .WithData("Email", email);
        }

        var member = new Member(
            GuidGenerator.Create(),
            firstName,
            lastName,
            membershipNumber,
            email,
            membershipType,
            phoneNumber,
            address);

        return member;
    }

    public async Task ChangeMembershipNumberAsync(Member member, string newMembershipNumber)
    {
        Check.NotNull(member, nameof(member));
        Check.NotNullOrWhiteSpace(newMembershipNumber, nameof(newMembershipNumber));

        if (await _memberRepository.IsMembershipNumberExistsAsync(newMembershipNumber, member.Id))
        {
            throw new BusinessException(LibraryManagementErrorCodes.DuplicateMembershipNumber)
                .WithData("MembershipNumber", newMembershipNumber);
        }

        member.SetMembershipNumber(newMembershipNumber);
    }

    public async Task ChangeEmailAsync(Member member, string newEmail)
    {
        Check.NotNull(member, nameof(member));
        Check.NotNullOrWhiteSpace(newEmail, nameof(newEmail));

        if (await _memberRepository.IsEmailExistsAsync(newEmail, member.Id))
        {
            throw new BusinessException(LibraryManagementErrorCodes.MemberAlreadyExists)
                .WithData("Email", newEmail);
        }

        member.SetEmail(newEmail);
    }

    public async Task ValidateMemberForLoanAsync(Member member)
    {
        Check.NotNull(member, nameof(member));

        // Check if membership is expired
        if (member.IsExpired())
        {
            throw new BusinessException(LibraryManagementErrorCodes.MembershipExpired)
                .WithData("MemberId", member.Id)
                .WithData("ExpiryDate", member.ExpiryDate);
        }

        // Check for overdue books
        if (await _loanRepository.HasOverdueBooksByMemberIdAsync(member.Id))
        {
            throw new BusinessException(LibraryManagementErrorCodes.MemberHasOverdueBooks)
                .WithData("MemberId", member.Id);
        }

        // Check loan limit
        var activeLoanCount = await _loanRepository.GetActiveLoanCountByMemberIdAsync(member.Id);
        if (activeLoanCount >= member.MaxLoanLimit)
        {
            throw new BusinessException(LibraryManagementErrorCodes.MaximumLoanLimitReached)
                .WithData("MemberId", member.Id)
                .WithData("MaxLoanLimit", member.MaxLoanLimit)
                .WithData("CurrentLoans", activeLoanCount);
        }
    }

    public async Task ValidateMemberDeletionAsync(Member member)
    {
        Check.NotNull(member, nameof(member));

        // Check if member has active loans
        var activeLoans = await _loanRepository.GetActiveLoansByMemberIdAsync(member.Id);
        if (activeLoans.Any())
        {
            throw new BusinessException(LibraryManagementErrorCodes.MemberHasActiveLoans)
                .WithData("MemberId", member.Id)
                .WithData("ActiveLoanCount", activeLoans.Count);
        }
    }
}
