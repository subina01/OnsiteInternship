using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LibraryManagement.Application.Contracts.DTOs.Books;

public class GetBookListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? AuthorId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? PublisherId { get; set; }
    public BookStatus? Status { get; set; }
    public int? PublicationYearFrom { get; set; }
    public int? PublicationYearTo { get; set; }
    public string? Language { get; set; }
    public bool? AvailableOnly { get; set; }
}
