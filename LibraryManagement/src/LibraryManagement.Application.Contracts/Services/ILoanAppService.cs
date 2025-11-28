using LibraryManagement.Application.Contracts.DTOs.Loans;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LibraryManagement.Application.Contracts.Services;

public interface ILoanAppService : IApplicationService
{
    Task<LoanDto> GetAsync(Guid id);
    Task<PagedResultDto<LoanDto>> GetListAsync(GetLoanListInput input);
    Task<LoanDto> CreateAsync(CreateLoanDto input);
    Task<LoanDto> RenewAsync(Guid id, RenewLoanDto input);
    Task<LoanDto> ReturnAsync(Guid id, ReturnLoanDto input);
    Task<LoanDto> MarkAsLostAsync(Guid id);
    Task<List<LoanDto>> GetActiveLoansAsync();
    Task<List<LoanDto>> GetOverdueLoansAsync();
    Task<List<LoanDto>> GetLoansByMemberAsync(Guid memberId);
    Task<List<LoanDto>> GetLoansByBookAsync(Guid bookId);
    Task UpdateOverdueLoansAsync();
}