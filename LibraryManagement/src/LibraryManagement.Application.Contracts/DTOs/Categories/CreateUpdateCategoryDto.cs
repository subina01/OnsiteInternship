using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Categories;

public class CreateUpdateCategoryDto
{
    [Required]
    [StringLength(LibraryManagementConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(LibraryManagementConsts.MaxDescriptionLength)]
    public string? Description { get; set; }
}
