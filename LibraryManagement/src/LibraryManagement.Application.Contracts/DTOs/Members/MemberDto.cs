using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LibraryManagement.Application.Contracts.DTOs.Members;

public class MemberDto : FullAuditedEntityDto<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string MembershipNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public MembershipType MembershipType { get; set; }
    public DateTime JoinDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int MaxLoanLimit { get; set; }
    public AddressDto? Address { get; set; }
    public bool IsExpired { get; set; }
    public int ActiveLoanCount { get; set; }
}