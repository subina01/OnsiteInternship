using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Shared.Constants;

/// <summary>
/// Contains constants used across the LibraryManagement domain
/// </summary>
public static class LibraryManagementConsts
{
    /// <summary>
    /// Default max length for name fields
    /// </summary>
    public const int MaxNameLength = 256;

    /// <summary>
    /// Default max length for title fields
    /// </summary>
    public const int MaxTitleLength = 512;

    /// <summary>
    /// Default max length for description fields
    /// </summary>
    public const int MaxDescriptionLength = 2048;

    /// <summary>
    /// Default max length for short text fields
    /// </summary>
    public const int MaxShortTextLength = 128;

    /// <summary>
    /// Default max length for email fields
    /// </summary>
    public const int MaxEmailLength = 256;

    /// <summary>
    /// Default max length for phone number fields
    /// </summary>
    public const int MaxPhoneNumberLength = 32;

    /// <summary>
    /// Default max length for URL fields
    /// </summary>
    public const int MaxUrlLength = 512;

    /// <summary>
    /// ISBN-10 length
    /// </summary>
    public const int Isbn10Length = 10;

    /// <summary>
    /// ISBN-13 length
    /// </summary>
    public const int Isbn13Length = 13;

    /// <summary>
    /// Maximum ISBN length (for validation)
    /// </summary>
    public const int MaxIsbnLength = 17; // ISBN-13 with hyphens

    public static class Books
    {
        /// <summary>
        /// Maximum quantity of copies for a single book
        /// </summary>
        public const int MaxQuantity = 1000;

        /// <summary>
        /// Minimum publication year
        /// </summary>
        public const int MinPublicationYear = 1000;

        /// <summary>
        /// Maximum page count
        /// </summary>
        public const int MaxPageCount = 10000;

        /// <summary>
        /// Minimum page count
        /// </summary>
        public const int MinPageCount = 1;

        /// <summary>
        /// Maximum edition number
        /// </summary>
        public const int MaxEdition = 100;
    }

    public static class Members
    {
        /// <summary>
        /// Maximum length for membership number
        /// </summary>
        public const int MaxMembershipNumberLength = 20;

        /// <summary>
        /// Default loan limit for standard membership
        /// </summary>
        public const int StandardLoanLimit = 5;

        /// <summary>
        /// Default loan limit for premium membership
        /// </summary>
        public const int PremiumLoanLimit = 10;

        /// <summary>
        /// Default loan limit for student membership
        /// </summary>
        public const int StudentLoanLimit = 5;

        /// <summary>
        /// Default loan limit for faculty membership
        /// </summary>
        public const int FacultyLoanLimit = 15;

        /// <summary>
        /// Default loan limit for senior membership
        /// </summary>
        public const int SeniorLoanLimit = 5;

        /// <summary>
        /// Membership duration in months for standard membership
        /// </summary>
        public const int StandardMembershipDurationMonths = 12;

        /// <summary>
        /// Membership duration in months for premium membership
        /// </summary>
        public const int PremiumMembershipDurationMonths = 24;
    }

    public static class Loans
    {
        /// <summary>
        /// Default loan duration in days
        /// </summary>
        public const int DefaultLoanDurationDays = 14;

        /// <summary>
        /// Maximum loan duration in days
        /// </summary>
        public const int MaxLoanDurationDays = 90;

        /// <summary>
        /// Maximum number of renewals allowed
        /// </summary>
        public const int MaxRenewalCount = 3;

        /// <summary>
        /// Grace period in days before applying overdue fines
        /// </summary>
        public const int GracePeriodDays = 3;

        /// <summary>
        /// Default daily fine amount
        /// </summary>
        public const decimal DefaultDailyFine = 0.50m;

        /// <summary>
        /// Maximum fine amount
        /// </summary>
        public const decimal MaxFineAmount = 100.00m;
    }

    public static class Reservations
    {
        /// <summary>
        /// Reservation expiration period in days
        /// </summary>
        public const int DefaultExpirationDays = 7;

        /// <summary>
        /// Maximum number of active reservations per member
        /// </summary>
        public const int MaxActiveReservationsPerMember = 5;

        /// <summary>
        /// Days to hold a book for pickup
        /// </summary>
        public const int HoldPeriodDays = 3;
    }

    public static class Authors
    {
        /// <summary>
        /// Maximum biography length
        /// </summary>
        public const int MaxBiographyLength = 4000;
    }

    public static class Address
    {
        /// <summary>
        /// Maximum street address length
        /// </summary>
        public const int MaxStreetLength = 256;

        /// <summary>
        /// Maximum city name length
        /// </summary>
        public const int MaxCityLength = 128;

        /// <summary>
        /// Maximum state/province name length
        /// </summary>
        public const int MaxStateLength = 128;

        /// <summary>
        /// Maximum ZIP/postal code length
        /// </summary>
        public const int MaxZipCodeLength = 20;

        /// <summary>
        /// Maximum country name length
        /// </summary>
        public const int MaxCountryLength = 128;
    }

    public static class DbTablePrefix
    {
        /// <summary>
        /// Database table prefix
        /// </summary>
        public const string Default = "Lib";
    }

    public static class DbSchema
    {
        /// <summary>
        /// Database schema name
        /// </summary>
        public const string Default = null;
    }
}