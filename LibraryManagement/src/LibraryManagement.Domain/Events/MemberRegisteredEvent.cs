using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Events;

/// <summary>
/// Domain Event - Raised when a new member is registered
/// </summary>
[Serializable]
public class MemberRegisteredEvent
{
    public Guid MemberId { get; }
    public string FullName { get; }
    public string Email { get; }
    public string MembershipNumber { get; }
    public MembershipType MembershipType { get; }
    public DateTime ExpiryDate { get; }

    public MemberRegisteredEvent(Member member)
    {
        MemberId = member.Id;
        FullName = member.FullName;
        Email = member.Email;
        MembershipNumber = member.MembershipNumber;
        MembershipType = member.MembershipType;
        ExpiryDate = member.ExpiryDate;
    }
}
