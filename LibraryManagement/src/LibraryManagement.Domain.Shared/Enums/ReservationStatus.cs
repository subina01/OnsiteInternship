using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Shared.Enums;

/// <summary>
/// Represents the status of a book reservation
/// </summary>
public enum ReservationStatus
{
    /// <summary>
    /// Reservation is pending
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Book is ready for pickup
    /// </summary>
    Ready = 1,

    /// <summary>
    /// Reservation has been fulfilled
    /// </summary>
    Fulfilled = 2,

    /// <summary>
    /// Reservation has been cancelled
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Reservation has expired
    /// </summary>
    Expired = 4
}
