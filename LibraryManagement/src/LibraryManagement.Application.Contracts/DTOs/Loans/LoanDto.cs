using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LibraryManagement.Application.Contracts.DTOs.Loans;

public class LoanDto : FullAuditedEntityDto<Guid>
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookISBN { get; set; } = string.Empty;

    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MembershipNumber { get; set; } = string.Empty;

    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public int RenewalCount { get; set; }
    public decimal Fine { get; set; }
    public string? Notes { get; set; }
    public bool IsOverdue { get; set; }
    public int OverdueDays { get; set; }
}
