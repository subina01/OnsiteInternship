using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Shared.Enums;

public enum MembershipType
{
    /// <summary>
    /// Standard membership with basic borrowing privileges
    /// </summary>
    Standard = 0,

    /// <summary>
    /// Premium membership with extended privileges
    /// </summary>
    Premium = 1,

    /// <summary>
    /// Student membership with special benefits
    /// </summary>
    Student = 2,

    /// <summary>
    /// Faculty membership for educational staff
    /// </summary>
    Faculty = 3,

    /// <summary>
    /// Senior citizen membership with special benefits
    /// </summary>
    Senior = 4
}
