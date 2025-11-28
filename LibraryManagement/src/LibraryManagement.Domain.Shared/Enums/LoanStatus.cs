using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Shared.Enums;

public enum LoanStatus
{
    /// <summary>
    /// Loan is currently active
    /// </summary>
    Active = 0,

    /// <summary>
    /// Book has been returned
    /// </summary>
    Returned = 1,

    /// <summary>
    /// Loan is past due date
    /// </summary>
    Overdue = 2,

    /// <summary>
    /// Book is reported as lost by borrower
    /// </summary>
    Lost = 3
}
