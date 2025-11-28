using LibraryManagement.Domain.Shared.Constants;
using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// Loan Aggregate Root - Represents a book loan transaction
/// </summary>
public class Loan : FullAuditedAggregateRoot<Guid>
{
    public Guid BookId { get; private set; }
    public Guid MemberId { get; private set; }
    public DateTime LoanDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; }
    public int RenewalCount { get; private set; }
    public decimal Fine { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public virtual Book Book { get; private set; } = null!;
    public virtual Member Member { get; private set; } = null!;

    private Loan()
    {
    }

    internal Loan(
        Guid id,
        Guid bookId,
        Guid memberId,
        DateTime loanDate,
        int loanDurationDays = LibraryManagementConsts.Loans.DefaultLoanDurationDays) : base(id)
    {
        BookId = bookId;
        MemberId = memberId;
        LoanDate = loanDate;
        DueDate = loanDate.AddDays(loanDurationDays);
        Status = LoanStatus.Active;
        RenewalCount = 0;
        Fine = 0;
    }

    public Loan Renew(int additionalDays = LibraryManagementConsts.Loans.DefaultLoanDurationDays)
    {
        if (Status != LoanStatus.Active)
        {
            throw new BusinessException(LibraryManagementErrorCodes.LoanNotActive)
                .WithData("LoanId", Id)
                .WithData("Status", Status);
        }

        if (IsOverdue())
        {
            throw new BusinessException(LibraryManagementErrorCodes.CannotRenewOverdueLoan)
                .WithData("LoanId", Id)
                .WithData("DueDate", DueDate);
        }

        if (RenewalCount >= LibraryManagementConsts.Loans.MaxRenewalCount)
        {
            throw new BusinessException(LibraryManagementErrorCodes.MaximumRenewalLimitReached)
                .WithData("LoanId", Id)
                .WithData("MaxRenewals", LibraryManagementConsts.Loans.MaxRenewalCount);
        }

        DueDate = DueDate.AddDays(additionalDays);
        RenewalCount++;

        return this;
    }

    public Loan Return(DateTime returnDate)
    {
        if (Status == LoanStatus.Returned)
        {
            throw new BusinessException(LibraryManagementErrorCodes.LoanAlreadyReturned)
                .WithData("LoanId", Id);
        }

        if (returnDate < LoanDate)
        {
            throw new BusinessException(LibraryManagementErrorCodes.ValidationError)
                .WithData("Field", "ReturnDate")
                .WithData("Message", "Return date cannot be before loan date");
        }

        ReturnDate = returnDate;
        Status = LoanStatus.Returned;

        // Calculate fine if overdue
        if (returnDate > DueDate)
        {
            CalculateFine(returnDate);
        }

        return this;
    }

    public Loan MarkAsLost()
    {
        if (Status == LoanStatus.Returned)
        {
            throw new BusinessException(LibraryManagementErrorCodes.LoanAlreadyReturned)
                .WithData("LoanId", Id);
        }

        Status = LoanStatus.Lost;
        Fine = LibraryManagementConsts.Loans.MaxFineAmount;

        return this;
    }

    public Loan SetNotes(string? notes)
    {
        if (!string.IsNullOrWhiteSpace(notes))
        {
            Check.Length(notes, nameof(notes), LibraryManagementConsts.MaxDescriptionLength);
        }
        Notes = notes?.Trim();
        return this;
    }

    public bool IsOverdue()
    {
        if (Status == LoanStatus.Returned)
        {
            return false;
        }

        return DateTime.UtcNow > DueDate;
    }

    public void UpdateStatus()
    {
        if (Status == LoanStatus.Returned || Status == LoanStatus.Lost)
        {
            return;
        }

        if (IsOverdue())
        {
            Status = LoanStatus.Overdue;
            CalculateFine(DateTime.UtcNow);
        }
    }

    private void CalculateFine(DateTime currentDate)
    {
        var overdueDays = (currentDate.Date - DueDate.Date).Days;

        if (overdueDays <= LibraryManagementConsts.Loans.GracePeriodDays)
        {
            Fine = 0;
            return;
        }

        var chargeableDays = overdueDays - LibraryManagementConsts.Loans.GracePeriodDays;
        Fine = chargeableDays * LibraryManagementConsts.Loans.DefaultDailyFine;

        if (Fine > LibraryManagementConsts.Loans.MaxFineAmount)
        {
            Fine = LibraryManagementConsts.Loans.MaxFineAmount;
        }
    }

    public int GetOverdueDays()
    {
        if (!IsOverdue())
        {
            return 0;
        }

        var compareDate = ReturnDate ?? DateTime.UtcNow;
        return (compareDate.Date - DueDate.Date).Days;
    }
}
