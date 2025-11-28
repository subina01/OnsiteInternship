using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Publishers;

public class CreateUpdatePublisherDto
{
    [Required]
    [StringLength(LibraryManagementConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(LibraryManagementConsts.MaxUrlLength)]
    [Url]
    public string? Website { get; set; }

    [StringLength(LibraryManagementConsts.MaxEmailLength)]
    [EmailAddress]
    public string? ContactEmail { get; set; }

    [StringLength(LibraryManagementConsts.MaxPhoneNumberLength)]
    [Phone]
    public string? ContactPhone { get; set; }
}
