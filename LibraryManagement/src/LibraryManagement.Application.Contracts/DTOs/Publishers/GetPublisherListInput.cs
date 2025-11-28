using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LibraryManagement.Application.Contracts.DTOs.Publishers;

public class GetPublisherListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
