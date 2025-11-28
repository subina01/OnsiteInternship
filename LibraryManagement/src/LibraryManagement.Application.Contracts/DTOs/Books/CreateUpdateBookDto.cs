using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagement.Application.Contracts.DTOs.Books;

public class CreateUpdateBookDto
{
    [Required]
    [StringLength(LibraryManagementConsts.MaxTitleLength)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(LibraryManagementConsts.MaxIsbnLength)]
    public string ISBN { get; set; } = string.Empty;

    [Required]
    [Range(LibraryManagementConsts.Books.MinPublicationYear, 2100)]
    public int PublicationYear { get; set; }

    [Required]
    [Range(1, LibraryManagementConsts.Books.MaxEdition)]
    public int Edition { get; set; }

    [StringLength(LibraryManagementConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [StringLength(LibraryManagementConsts.MaxShortTextLength)]
    public string? Language { get; set; }

    [Range(LibraryManagementConsts.Books.MinPageCount, LibraryManagementConsts.Books.MaxPageCount)]
    public int? PageCount { get; set; }

    [Required]
    [Range(0, LibraryManagementConsts.Books.MaxQuantity)]
    public int Quantity { get; set; }

    public Guid? PublisherId { get; set; }

    public List<Guid> AuthorIds { get; set; } = new();
    public List<Guid> CategoryIds { get; set; } = new();
}
