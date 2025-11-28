using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Events;

/// <summary>
/// Domain Event - Raised when a new reservation is created
/// </summary>
[Serializable]
public class ReservationCreatedEvent
{
    public Guid ReservationId { get; }
    public Guid BookId { get; }
    public Guid MemberId { get; }
    public DateTime ReservationDate { get; }
    public DateTime ExpirationDate { get; }

    public ReservationCreatedEvent(Reservation reservation)
    {
        ReservationId = reservation.Id;
        BookId = reservation.BookId;
        MemberId = reservation.MemberId;
        ReservationDate = reservation.ReservationDate;
        ExpirationDate = reservation.ExpirationDate;
    }
}
