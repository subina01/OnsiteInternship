using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Shared.Enums;

/// <summary>
/// Represents the status of a book in the library inventory
/// </summary>
public enum BookStatus
{
    /// <summary>
    /// Book is available for loan
    /// </summary>
    Available = 0,

    /// <summary>
    /// Book is currently checked out
    /// </summary>
    CheckedOut = 1,

    /// <summary>
    /// Book is reserved for a member
    /// </summary>
    Reserved = 2,

    /// <summary>
    /// Book is under maintenance or repair
    /// </summary>
    Maintenance = 3,

    /// <summary>
    /// Book is reported as lost
    /// </summary>
    Lost = 4,

    /// <summary>
    /// Book is damaged and not available
    /// </summary>
    Damaged = 5
}
