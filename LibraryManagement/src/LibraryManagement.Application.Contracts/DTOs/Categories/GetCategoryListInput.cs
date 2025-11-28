using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LibraryManagement.Application.Contracts.DTOs.Categories;

public class GetCategoryListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
