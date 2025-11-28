using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Events;

/// <summary>
/// Domain Event - Raised when a new loan is created
/// </summary>
[Serializable]
public class LoanCreatedEvent
{
    public Guid LoanId { get; }
    public Guid BookId { get; }
    public Guid MemberId { get; }
    public DateTime LoanDate { get; }
    public DateTime DueDate { get; }

    public LoanCreatedEvent(Loan loan)
    {
        LoanId = loan.Id;
        BookId = loan.BookId;
        MemberId = loan.MemberId;
        LoanDate = loan.LoanDate;
        DueDate = loan.DueDate;
    }
}
