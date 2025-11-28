using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Loans;

public class ReturnLoanDto
{
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;

    [StringLength(LibraryManagementConsts.MaxDescriptionLength)]
    public string? Notes { get; set; }
}

