using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.EntityFrameworkCore.EntityFrameworkCore;

namespace LibraryManagement.EntityFrameworkCore.Repositories;

public class EfCoreLoanRepository : EfCoreRepository<LibraryManagementDbContext, Loan, Guid>, ILoanRepository
{
    public EfCoreLoanRepository(IDbContextProvider<LibraryManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Loan>> GetActiveLoansAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Status == LoanStatus.Active || x.Status == LoanStatus.Overdue)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var now = DateTime.UtcNow;
        return await dbSet
            .Where(x => (x.Status == LoanStatus.Active || x.Status == LoanStatus.Overdue)
                       && x.DueDate < now)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Loan>> GetLoansByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.MemberId == memberId)
            .OrderByDescending(x => x.LoanDate)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Loan>> GetActiveLoansByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.MemberId == memberId
                       && (x.Status == LoanStatus.Active || x.Status == LoanStatus.Overdue))
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Loan>> GetLoansByBookIdAsync(
        Guid bookId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.BookId == bookId)
            .OrderByDescending(x => x.LoanDate)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<int> GetActiveLoanCountByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .CountAsync(
                x => x.MemberId == memberId
                    && (x.Status == LoanStatus.Active || x.Status == LoanStatus.Overdue),
                GetCancellationToken(cancellationToken));
    }

    public async Task<bool> HasOverdueBooksByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var now = DateTime.UtcNow;
        return await dbSet
            .AnyAsync(
                x => x.MemberId == memberId
                    && (x.Status == LoanStatus.Active || x.Status == LoanStatus.Overdue)
                    && x.DueDate < now,
                GetCancellationToken(cancellationToken));
    }

    public override async Task<IQueryable<Loan>> WithDetailsAsync()
    {
        return (await GetQueryableAsync())
            .Include(x => x.Book)
            .Include(x => x.Member);
    }
}
