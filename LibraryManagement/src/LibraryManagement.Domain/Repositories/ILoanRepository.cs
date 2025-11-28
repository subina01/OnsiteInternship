using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace LibraryManagement.Domain.Repositories;

/// <summary>
/// Repository interface for Loan aggregate
/// </summary>
public interface ILoanRepository : IRepository<Loan, Guid>
{
    Task<List<Loan>> GetActiveLoansAsync(
        CancellationToken cancellationToken = default);

    Task<List<Loan>> GetOverdueLoansAsync(
        CancellationToken cancellationToken = default);

    Task<List<Loan>> GetLoansByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default);

    Task<List<Loan>> GetActiveLoansByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default);

    Task<List<Loan>> GetLoansByBookIdAsync(
        Guid bookId,
        CancellationToken cancellationToken = default);

    Task<int> GetActiveLoanCountByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverdueBooksByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default);
}
