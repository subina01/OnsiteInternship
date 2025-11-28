using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace LibraryManagement.Domain.DomainServices;

/// <summary>
/// Domain Service for managing Loan business logic
/// </summary>
public class LoanManager : DomainService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly BookManager _bookManager;
    private readonly MemberManager _memberManager;

    public LoanManager(
        ILoanRepository loanRepository,
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        BookManager bookManager,
        MemberManager memberManager)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _bookManager = bookManager;
        _memberManager = memberManager;
    }

    public async Task<Loan> CreateLoanAsync(
        Guid bookId,
        Guid memberId,
        int loanDurationDays = LibraryManagementConsts.Loans.DefaultLoanDurationDays)
    {
        // Get book and validate
        var book = await _bookRepository.GetAsync(bookId);
        _bookManager.ValidateBookAvailability(book);

        // Get member and validate
        var member = await _memberRepository.GetAsync(memberId);
        await _memberManager.ValidateMemberForLoanAsync(member);

        // Create loan
        var loan = new Loan(
            GuidGenerator.Create(),
            bookId,
            memberId,
            DateTime.UtcNow,
            loanDurationDays);

        // Decrease available quantity
        book.DecreaseAvailableQuantity();
        await _bookRepository.UpdateAsync(book);

        return loan;
    }

    public async Task<Loan> ReturnLoanAsync(Guid loanId, DateTime returnDate)
    {
        var loan = await _loanRepository.GetAsync(loanId);

        // Mark loan as returned
        loan.Return(returnDate);

        // Increase available quantity
        var book = await _bookRepository.GetAsync(loan.BookId);
        book.IncreaseAvailableQuantity();
        await _bookRepository.UpdateAsync(book);

        return loan;
    }

    public async Task<Loan> RenewLoanAsync(
        Guid loanId,
        int additionalDays = LibraryManagementConsts.Loans.DefaultLoanDurationDays)
    {
        var loan = await _loanRepository.GetAsync(loanId);

        // Validate member still eligible
        var member = await _memberRepository.GetAsync(loan.MemberId);
        if (member.IsExpired())
        {
            throw new BusinessException(LibraryManagementErrorCodes.MembershipExpired)
                .WithData("MemberId", member.Id);
        }

        // Renew the loan
        loan.Renew(additionalDays);

        return loan;
    }

    public async Task UpdateOverdueLoansAsync()
    {
        var activeLoans = await _loanRepository.GetActiveLoansAsync();

        foreach (var loan in activeLoans)
        {
            loan.UpdateStatus();
        }

        await _loanRepository.UpdateManyAsync(activeLoans);
    }
}
