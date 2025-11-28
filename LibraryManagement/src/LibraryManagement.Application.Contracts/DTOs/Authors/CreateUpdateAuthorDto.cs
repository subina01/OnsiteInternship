using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Authors;

public class CreateUpdateAuthorDto
{
    [Required]
    [StringLength(LibraryManagementConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(LibraryManagementConsts.Authors.MaxBiographyLength)]
    public string? Biography { get; set; }

    public DateTime? BirthDate { get; set; }

    [StringLength(LibraryManagementConsts.MaxShortTextLength)]
    public string? Nationality { get; set; }
}
