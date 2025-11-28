using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LibraryManagement.Application.Contracts.DTOs.Authors;

public class GetAuthorListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public string? Nationality { get; set; }
}
