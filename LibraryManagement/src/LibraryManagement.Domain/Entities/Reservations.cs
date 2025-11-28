using LibraryManagement.Domain.Shared.Constants;
using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// Reservation Aggregate Root - Represents a book reservation
/// </summary>
public class Reservation : FullAuditedAggregateRoot<Guid>
{
    public Guid BookId { get; private set; }
    public Guid MemberId { get; private set; }
    public DateTime ReservationDate { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime? ReadyForPickupDate { get; private set; }
    public DateTime? FulfilledDate { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public virtual Book Book { get; private set; } = null!;
    public virtual Member Member { get; private set; } = null!;

    private Reservation()
    {
    }

    public Reservation(
        Guid id,
        Guid bookId,
        Guid memberId,
        DateTime reservationDate) : base(id)
    {
        BookId = bookId;
        MemberId = memberId;
        ReservationDate = reservationDate;
        ExpirationDate = reservationDate.AddDays(LibraryManagementConsts.Reservations.DefaultExpirationDays);
        Status = ReservationStatus.Pending;
    }

    public Reservation MarkAsReady()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new BusinessException(LibraryManagementErrorCodes.InvalidOperation)
                .WithData("Message", "Only pending reservations can be marked as ready");
        }

        Status = ReservationStatus.Ready;
        ReadyForPickupDate = DateTime.UtcNow;
        ExpirationDate = ReadyForPickupDate.Value.AddDays(LibraryManagementConsts.Reservations.HoldPeriodDays);

        return this;
    }

    public Reservation Fulfill()
    {
        if (Status != ReservationStatus.Ready)
        {
            throw new BusinessException(LibraryManagementErrorCodes.InvalidOperation)
                .WithData("Message", "Only ready reservations can be fulfilled");
        }

        Status = ReservationStatus.Fulfilled;
        FulfilledDate = DateTime.UtcNow;

        return this;
    }

    public Reservation Cancel()
    {
        if (Status == ReservationStatus.Fulfilled)
        {
            throw new BusinessException(LibraryManagementErrorCodes.InvalidOperation)
                .WithData("Message", "Fulfilled reservations cannot be cancelled");
        }

        if (Status == ReservationStatus.Cancelled)
        {
            throw new BusinessException(LibraryManagementErrorCodes.ReservationAlreadyCancelled)
                .WithData("ReservationId", Id);
        }

        Status = ReservationStatus.Cancelled;

        return this;
    }

    public Reservation MarkAsExpired()
    {
        if (Status == ReservationStatus.Fulfilled || Status == ReservationStatus.Cancelled)
        {
            throw new BusinessException(LibraryManagementErrorCodes.InvalidOperation)
                .WithData("Message", "Cannot expire fulfilled or cancelled reservations");
        }

        Status = ReservationStatus.Expired;

        return this;
    }

    public Reservation SetNotes(string? notes)
    {
        if (!string.IsNullOrWhiteSpace(notes))
        {
            Check.Length(notes, nameof(notes), LibraryManagementConsts.MaxDescriptionLength);
        }
        Notes = notes?.Trim();
        return this;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpirationDate &&
               Status != ReservationStatus.Fulfilled &&
               Status != ReservationStatus.Cancelled;
    }

    public bool IsActive()
    {
        return Status == ReservationStatus.Pending || Status == ReservationStatus.Ready;
    }

    public void UpdateStatus()
    {
        if (IsExpired() && Status != ReservationStatus.Expired)
        {
            MarkAsExpired();
        }
    }
}