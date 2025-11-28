using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Loans;

public class RenewLoanDto
{
    [Range(1, LibraryManagementConsts.Loans.MaxLoanDurationDays)]
    public int AdditionalDays { get; set; } = LibraryManagementConsts.Loans.DefaultLoanDurationDays;
}
