using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// Category Entity - Represents a book category/genre
/// </summary>
public class Category : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }

    // Navigation property
    public virtual ICollection<BookCategory> BookCategories { get; private set; }

    private Category()
    {
        Name = string.Empty;
        BookCategories = new List<BookCategory>();
    }

    public Category(
        Guid id,
        string name,
        string? description = null) : base(id)
    {
        Name = string.Empty; // Ensure non-null before calling SetName
        SetName(name);
        Description = description;
        BookCategories = new List<BookCategory>();
    }

    public Category ChangeName(string name)
    {
        SetName(name);
        return this;
    }

    public Category SetDescription(string? description)
    {
        if (!string.IsNullOrWhiteSpace(description))
        {
            Check.Length(description, nameof(description), LibraryManagementConsts.MaxDescriptionLength);
        }
        Description = description?.Trim();
        return this;
    }

    private void SetName(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Length(name, nameof(name), LibraryManagementConsts.MaxNameLength);
        Name = name.Trim();
    }
}
