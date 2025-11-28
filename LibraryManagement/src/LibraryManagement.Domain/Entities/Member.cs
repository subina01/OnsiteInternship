using LibraryManagement.Domain.Shared.Constants;
using LibraryManagement.Domain.Shared.Enums;
using LibraryManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// Member Aggregate Root - Represents a library member
/// </summary>
public class Member : FullAuditedAggregateRoot<Guid>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string MembershipNumber { get; private set; }
    public string Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public MembershipType MembershipType { get; private set; }
    public DateTime JoinDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public Address? Address { get; private set; }
    public int MaxLoanLimit { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    private Member()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        MembershipNumber = string.Empty;
        Email = string.Empty;
    }

    internal Member(
        Guid id,
        string firstName,
        string lastName,
        string membershipNumber,
        string email,
        MembershipType membershipType,
        string? phoneNumber = null,
        Address? address = null) : base(id)
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        MembershipNumber = string.Empty;
        Email = string.Empty;
        SetFirstName(firstName);
        SetLastName(lastName);
        SetMembershipNumber(membershipNumber);
        SetEmail(email);
        SetPhoneNumber(phoneNumber);
        SetMembershipType(membershipType);
        JoinDate = DateTime.UtcNow;
        CalculateExpiryDate();
        Address = address;
    }

    public Member SetFirstName(string firstName)
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName));
        Check.Length(firstName, nameof(firstName), LibraryManagementConsts.MaxNameLength);
        FirstName = firstName.Trim();
        return this;
    }

    public Member SetLastName(string lastName)
    {
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName));
        Check.Length(lastName, nameof(lastName), LibraryManagementConsts.MaxNameLength);
        LastName = lastName.Trim();
        return this;
    }

    public Member SetMembershipNumber(string membershipNumber)
    {
        Check.NotNullOrWhiteSpace(membershipNumber, nameof(membershipNumber));
        Check.Length(membershipNumber, nameof(membershipNumber),
            LibraryManagementConsts.Members.MaxMembershipNumberLength);
        MembershipNumber = membershipNumber.Trim().ToUpperInvariant();
        return this;
    }

    public Member SetEmail(string email)
    {
        Check.NotNullOrWhiteSpace(email, nameof(email));
        Check.Length(email, nameof(email), LibraryManagementConsts.MaxEmailLength);
        Email = email.Trim().ToLowerInvariant();
        return this;
    }

    public Member SetPhoneNumber(string? phoneNumber)
    {
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            Check.Length(phoneNumber, nameof(phoneNumber), LibraryManagementConsts.MaxPhoneNumberLength);
        }
        PhoneNumber = phoneNumber?.Trim();
        return this;
    }

    public Member SetMembershipType(MembershipType membershipType)
    {
        MembershipType = membershipType;
        MaxLoanLimit = GetLoanLimitForMembershipType(membershipType);
        CalculateExpiryDate();
        return this;
    }

    public Member SetAddress(Address? address)
    {
        Address = address;
        return this;
    }

    public Member RenewMembership()
    {
        if (!IsExpired())
        {
            throw new BusinessException(LibraryManagementErrorCodes.InvalidOperation)
                .WithData("Message", "Membership is still active and cannot be renewed yet");
        }

        JoinDate = DateTime.UtcNow;
        CalculateExpiryDate();
        return this;
    }

    public Member ExtendMembership(int months)
    {
        if (months <= 0)
        {
            throw new BusinessException(LibraryManagementErrorCodes.ValidationError)
                .WithData("Field", "Months")
                .WithData("Message", "Extension period must be positive");
        }

        ExpiryDate = ExpiryDate.AddMonths(months);
        return this;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiryDate;
    }

    public bool IsActive()
    {
        return !IsExpired() && !IsDeleted;
    }

    private void CalculateExpiryDate()
    {
        var durationMonths = MembershipType switch
        {
            MembershipType.Premium => LibraryManagementConsts.Members.PremiumMembershipDurationMonths,
            _ => LibraryManagementConsts.Members.StandardMembershipDurationMonths
        };

        ExpiryDate = JoinDate.AddMonths(durationMonths);
    }

    private static int GetLoanLimitForMembershipType(MembershipType membershipType)
    {
        return membershipType switch
        {
            MembershipType.Standard => LibraryManagementConsts.Members.StandardLoanLimit,
            MembershipType.Premium => LibraryManagementConsts.Members.PremiumLoanLimit,
            MembershipType.Student => LibraryManagementConsts.Members.StudentLoanLimit,
            MembershipType.Faculty => LibraryManagementConsts.Members.FacultyLoanLimit,
            MembershipType.Senior => LibraryManagementConsts.Members.SeniorLoanLimit,
            _ => LibraryManagementConsts.Members.StandardLoanLimit
        };
    }
}