using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LibraryManagement.Application.Contracts.DTOs.Books;

public class BookDto : FullAuditedEntityDto<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public int Edition { get; set; }
    public string? Description { get; set; }
    public string? Language { get; set; }
    public int? PageCount { get; set; }
    public int Quantity { get; set; }
    public int AvailableQuantity { get; set; }
    public BookStatus Status { get; set; }
    public Guid? PublisherId { get; set; }
    public string? PublisherName { get; set; }

    // Related data
    public List<AuthorBasicDto> Authors { get; set; } = new();
    public List<CategoryBasicDto> Categories { get; set; } = new();
}

public class AuthorBasicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CategoryBasicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
