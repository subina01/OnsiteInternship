using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Shared.Constants;

/// <summary>
/// Error codes for domain exceptions
/// </summary>
public static class LibraryManagementErrorCodes
{
    // Book Error Codes (1xxx)
    public const string BookNotFound = "LibraryManagement:1001";
    public const string BookNotAvailable = "LibraryManagement:1002";
    public const string DuplicateIsbn = "LibraryManagement:1003";
    public const string InvalidIsbn = "LibraryManagement:1004";
    public const string InsufficientBookQuantity = "LibraryManagement:1005";
    public const string BookAlreadyExists = "LibraryManagement:1006";

    // Member Error Codes (2xxx)
    public const string MemberNotFound = "LibraryManagement:2001";
    public const string MembershipExpired = "LibraryManagement:2002";
    public const string DuplicateMembershipNumber = "LibraryManagement:2003";
    public const string MemberHasOverdueBooks = "LibraryManagement:2004";
    public const string MaximumLoanLimitReached = "LibraryManagement:2005";
    public const string MemberAlreadyExists = "LibraryManagement:2006";
    public const string MemberHasActiveLoans = "LibraryManagement:2007";

    // Loan Error Codes (3xxx)
    public const string LoanNotFound = "LibraryManagement:3001";
    public const string LoanAlreadyReturned = "LibraryManagement:3002";
    public const string MaximumRenewalLimitReached = "LibraryManagement:3003";
    public const string CannotRenewOverdueLoan = "LibraryManagement:3004";
    public const string LoanNotActive = "LibraryManagement:3005";
    public const string CannotDeleteActiveLoan = "LibraryManagement:3006";

    // Reservation Error Codes (4xxx)
    public const string ReservationNotFound = "LibraryManagement:4001";
    public const string BookAlreadyReserved = "LibraryManagement:4002";
    public const string MaximumReservationLimitReached = "LibraryManagement:4003";
    public const string ReservationExpired = "LibraryManagement:4004";
    public const string ReservationAlreadyCancelled = "LibraryManagement:4005";
    public const string CannotReserveAvailableBook = "LibraryManagement:4006";

    // Author Error Codes (5xxx)
    public const string AuthorNotFound = "LibraryManagement:5001";
    public const string AuthorAlreadyExists = "LibraryManagement:5002";
    public const string AuthorHasBooks = "LibraryManagement:5003";

    // Category Error Codes (6xxx)
    public const string CategoryNotFound = "LibraryManagement:6001";
    public const string CategoryAlreadyExists = "LibraryManagement:6002";
    public const string CategoryHasBooks = "LibraryManagement:6003";

    // Publisher Error Codes (7xxx)
    public const string PublisherNotFound = "LibraryManagement:7001";
    public const string PublisherAlreadyExists = "LibraryManagement:7002";
    public const string PublisherHasBooks = "LibraryManagement:7003";

    // General Error Codes (9xxx)
    public const string InvalidOperation = "LibraryManagement:9001";
    public const string ConcurrencyConflict = "LibraryManagement:9002";
    public const string ValidationError = "LibraryManagement:9003";
}