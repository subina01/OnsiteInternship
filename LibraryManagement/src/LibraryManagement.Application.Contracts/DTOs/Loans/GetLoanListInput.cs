using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LibraryManagement.Application.Contracts.DTOs.Loans;

public class GetLoanListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? BookId { get; set; }
    public Guid? MemberId { get; set; }
    public LoanStatus? Status { get; set; }
    public bool? IsOverdue { get; set; }
    public DateTime? LoanDateFrom { get; set; }
    public DateTime? LoanDateTo { get; set; }
}
