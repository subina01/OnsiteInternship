using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Events;

/// <summary>
/// Domain Event - Raised when a loan is returned
/// </summary>
[Serializable]
public class LoanReturnedEvent
{
    public Guid LoanId { get; }
    public Guid BookId { get; }
    public Guid MemberId { get; }
    public DateTime ReturnDate { get; }
    public decimal Fine { get; }
    public bool WasOverdue { get; }

    public LoanReturnedEvent(Loan loan)
    {
        LoanId = loan.Id;
        BookId = loan.BookId;
        MemberId = loan.MemberId;
        ReturnDate = loan.ReturnDate!.Value;
        Fine = loan.Fine;
        WasOverdue = loan.Fine > 0;
    }
}
