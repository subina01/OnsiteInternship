using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LibraryManagement.Application.Contracts.DTOs.Members;

public class GetMemberListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public MembershipType? MembershipType { get; set; }
    public bool? IsExpired { get; set; }
    public bool? HasActiveLoans { get; set; }
}
