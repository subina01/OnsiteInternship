using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Loans;

public class CreateLoanDto
{
    [Required]
    public Guid BookId { get; set; }

    [Required]
    public Guid MemberId { get; set; }

    [Range(1, LibraryManagementConsts.Loans.MaxLoanDurationDays)]
    public int LoanDurationDays { get; set; } = LibraryManagementConsts.Loans.DefaultLoanDurationDays;

    [StringLength(LibraryManagementConsts.MaxDescriptionLength)]
    public string? Notes { get; set; }
}
