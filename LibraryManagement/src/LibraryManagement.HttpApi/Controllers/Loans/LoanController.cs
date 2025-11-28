using Asp.Versioning;
using LibraryManagement.Application.Contracts.DTOs.Loans;
using LibraryManagement.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace LibraryManagement.HttpApi.Controllers.Loans;

[RemoteService(Name = "LibraryManagement")]
[Area("libraryManagement")]
[ControllerName("Loan")]
[Route("api/library-management/loans")]
public class LoanController : AbpController
{
    private readonly ILoanAppService _loanAppService;

    public LoanController(ILoanAppService loanAppService)
    {
        _loanAppService = loanAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<LoanDto> GetAsync(Guid id)
    {
        return _loanAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<LoanDto>> GetListAsync(GetLoanListInput input)
    {
        return _loanAppService.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<LoanDto> CreateAsync(CreateLoanDto input)
    {
        return _loanAppService.CreateAsync(input);
    }

    [HttpPost]
    [Route("{id}/renew")]
    public virtual Task<LoanDto> RenewAsync(Guid id, RenewLoanDto input)
    {
        return _loanAppService.RenewAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/return")]
    public virtual Task<LoanDto> ReturnAsync(Guid id, ReturnLoanDto input)
    {
        return _loanAppService.ReturnAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/mark-as-lost")]
    public virtual Task<LoanDto> MarkAsLostAsync(Guid id)
    {
        return _loanAppService.MarkAsLostAsync(id);
    }

    [HttpGet]
    [Route("active")]
    public virtual Task<List<LoanDto>> GetActiveLoansAsync()
    {
        return _loanAppService.GetActiveLoansAsync();
    }

    [HttpGet]
    [Route("overdue")]
    public virtual Task<List<LoanDto>> GetOverdueLoansAsync()
    {
        return _loanAppService.GetOverdueLoansAsync();
    }

    [HttpGet]
    [Route("by-member/{memberId}")]
    public virtual Task<List<LoanDto>> GetLoansByMemberAsync(Guid memberId)
    {
        return _loanAppService.GetLoansByMemberAsync(memberId);
    }

    [HttpGet]
    [Route("by-book/{bookId}")]
    public virtual Task<List<LoanDto>> GetLoansByBookAsync(Guid bookId)
    {
        return _loanAppService.GetLoansByBookAsync(bookId);
    }

    [HttpPost]
    [Route("update-overdue")]
    public virtual Task UpdateOverdueLoansAsync()
    {
        return _loanAppService.UpdateOverdueLoansAsync();
    }
}
