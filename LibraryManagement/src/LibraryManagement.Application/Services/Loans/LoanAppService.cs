using AutoMapper.Internal.Mappers;
using LibraryManagement.Application.Contracts.DTOs.Loans;
using LibraryManagement.Application.Contracts.Permissions;
using LibraryManagement.Application.Contracts.Services;
using LibraryManagement.Domain.DomainServices;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Dynamic.Core;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LibraryManagement.Application.Services.Loans;

public class LoanAppService : ApplicationService, ILoanAppService
{
    private readonly ILoanRepository _loanRepository;
    private readonly LoanManager _loanManager;

    public LoanAppService(
        ILoanRepository loanRepository,
        LoanManager loanManager)
    {
        _loanRepository = loanRepository;
        _loanManager = loanManager;
    }

    public async Task<LoanDto> GetAsync(Guid id)
    {
        var loan = await _loanRepository.GetAsync(id, includeDetails: true);
        return ObjectMapper.Map<Loan, LoanDto>(loan);
    }

    public async Task<PagedResultDto<LoanDto>> GetListAsync(GetLoanListInput input)
    {
        var query = await _loanRepository.WithDetailsAsync();

        query = query
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                x => x.Member.FirstName.Contains(input.Filter!) ||
                     x.Member.LastName.Contains(input.Filter!) ||
                     x.Book.Title.Contains(input.Filter!))
            .WhereIf(input.BookId.HasValue,
                x => x.BookId == input.BookId!.Value)
            .WhereIf(input.MemberId.HasValue,
                x => x.MemberId == input.MemberId!.Value)
            .WhereIf(input.Status.HasValue,
                x => x.Status == input.Status!.Value)
            .WhereIf(input.IsOverdue == true,
                x => x.DueDate < DateTime.UtcNow && x.ReturnDate == null)
            .WhereIf(input.LoanDateFrom.HasValue,
                x => x.LoanDate >= input.LoanDateFrom!.Value)
            .WhereIf(input.LoanDateTo.HasValue,
                x => x.LoanDate <= input.LoanDateTo!.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = ApplySorting(query, input);
        query = ApplyPaging(query, input);

        var loans = await AsyncExecuter.ToListAsync(query);
        var loanDtos = ObjectMapper.Map<List<Loan>, List<LoanDto>>(loans);

        return new PagedResultDto<LoanDto>(totalCount, loanDtos);
    }

    public async Task<LoanDto> CreateAsync(CreateLoanDto input)
    {
        var loan = await _loanManager.CreateLoanAsync(
            input.BookId,
            input.MemberId,
            input.LoanDurationDays);

        if (!input.Notes.IsNullOrWhiteSpace())
        {
            loan.SetNotes(input.Notes);
        }

        await _loanRepository.InsertAsync(loan);

        return await GetAsync(loan.Id);
    }

    public async Task<LoanDto> RenewAsync(Guid id, RenewLoanDto input)
    {
        var loan = await _loanManager.RenewLoanAsync(id, input.AdditionalDays);
        await _loanRepository.UpdateAsync(loan);
        return await GetAsync(id);
    }

    public async Task<LoanDto> ReturnAsync(Guid id, ReturnLoanDto input)
    {
        var loan = await _loanManager.ReturnLoanAsync(id, input.ReturnDate);

        if (!input.Notes.IsNullOrWhiteSpace())
        {
            loan.SetNotes(input.Notes);
        }

        await _loanRepository.UpdateAsync(loan);
        return await GetAsync(id);
    }

    public async Task<LoanDto> MarkAsLostAsync(Guid id)
    {
        var loan = await _loanRepository.GetAsync(id);
        loan.MarkAsLost();
        await _loanRepository.UpdateAsync(loan);
        return await GetAsync(id);
    }

    public async Task<List<LoanDto>> GetActiveLoansAsync()
    {
        var loans = await _loanRepository.GetActiveLoansAsync();
        return ObjectMapper.Map<List<Loan>, List<LoanDto>>(loans);
    }

    public async Task<List<LoanDto>> GetOverdueLoansAsync()
    {
        var loans = await _loanRepository.GetOverdueLoansAsync();
        return ObjectMapper.Map<List<Loan>, List<LoanDto>>(loans);
    }

    public async Task<List<LoanDto>> GetLoansByMemberAsync(Guid memberId)
    {
        var loans = await _loanRepository.GetLoansByMemberIdAsync(memberId);
        return ObjectMapper.Map<List<Loan>, List<LoanDto>>(loans);
    }

    public async Task<List<LoanDto>> GetLoansByBookAsync(Guid bookId)
    {
        var loans = await _loanRepository.GetLoansByBookIdAsync(bookId);
        return ObjectMapper.Map<List<Loan>, List<LoanDto>>(loans);
    }

    public async Task UpdateOverdueLoansAsync()
    {
        await _loanManager.UpdateOverdueLoansAsync();
    }

    protected virtual IQueryable<Loan> ApplySorting(IQueryable<Loan> query, GetLoanListInput input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            return query.OrderByDescending(x => x.LoanDate);
        }

        // Use Dynamic LINQ for string-based sorting
        return query.OrderBy(input.Sorting);
    }

    protected virtual IQueryable<Loan> ApplyPaging(IQueryable<Loan> query, GetLoanListInput input)
    {
        return query.Skip(input.SkipCount).Take(input.MaxResultCount);
    }
}
