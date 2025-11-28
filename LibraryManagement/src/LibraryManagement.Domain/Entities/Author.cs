using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// Author Entity - Represents a book author
/// Inherits from FullAuditedAggregateRoot for complete audit trail
/// </summary>
public class Author : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string? Biography { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string? Nationality { get; private set; }

    // Navigation property for books
    public virtual ICollection<BookAuthor> BookAuthors { get; private set; }

    private Author()
    {
        Name = string.Empty;
        BookAuthors = new List<BookAuthor>();
    }

    public Author(
        Guid id,
        string name,
        string? biography = null,
        DateTime? birthDate = null,
        string? nationality = null) : base(id)
    {
        SetName(name);
        Biography = biography;
        BirthDate = birthDate;
        Nationality = nationality;
        BookAuthors = new List<BookAuthor>();
    }

    public Author ChangeName(string name)
    {
        SetName(name);
        return this;
    }

    public Author SetBiography(string? biography)
    {
        if (!string.IsNullOrWhiteSpace(biography))
        {
            Check.Length(biography, nameof(biography), LibraryManagementConsts.Authors.MaxBiographyLength);
        }
        Biography = biography?.Trim();
        return this;
    }

    public Author SetBirthDate(DateTime? birthDate)
    {
        if (birthDate.HasValue && birthDate.Value > DateTime.UtcNow)
        {
            throw new BusinessException(LibraryManagementErrorCodes.ValidationError)
                .WithData("Field", "BirthDate")
                .WithData("Message", "Birth date cannot be in the future");
        }
        BirthDate = birthDate;
        return this;
    }

    public Author SetNationality(string? nationality)
    {
        if (!string.IsNullOrWhiteSpace(nationality))
        {
            Check.Length(nationality, nameof(nationality), LibraryManagementConsts.MaxShortTextLength);
        }
        Nationality = nationality?.Trim();
        return this;
    }

    private void SetName(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Length(name, nameof(name), LibraryManagementConsts.MaxNameLength);
        Name = name.Trim();
    }
}
