using LibraryManagement.Domain.Shared.Constants;
using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Members;

public class CreateUpdateMemberDto
{
    [Required]
    [StringLength(LibraryManagementConsts.MaxNameLength)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(LibraryManagementConsts.MaxNameLength)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(LibraryManagementConsts.Members.MaxMembershipNumberLength)]
    public string MembershipNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(LibraryManagementConsts.MaxEmailLength)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(LibraryManagementConsts.MaxPhoneNumberLength)]
    public string? PhoneNumber { get; set; }

    [Required]
    public MembershipType MembershipType { get; set; }

    public CreateUpdateAddressDto? Address { get; set; }
}
