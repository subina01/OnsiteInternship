using LibraryManagement.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using static LibraryManagement.Domain.Shared.Constants.LibraryManagementConsts;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// Publisher Entity - Represents a book publisher
/// </summary>
public class Publisher : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string? Website { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }

    // Navigation property
    public virtual ICollection<Book> Books { get; private set; }

    private Publisher()
    {
        Name = string.Empty;
        Books = new List<Book>();
    }

    public Publisher(
        Guid id,
        string name,
        string? website = null,
        string? contactEmail = null,
        string? contactPhone = null) : base(id)
    {
        Name = string.Empty;
        SetName(name);
        SetWebsite(website);
        SetContactEmail(contactEmail);
        SetContactPhone(contactPhone);
        Books = new List<Book>();
    }

    public Publisher ChangeName(string name)
    {
        SetName(name);
        return this;
    }

    public Publisher SetWebsite(string? website)
    {
        if (!string.IsNullOrWhiteSpace(website))
        {
            Check.Length(website, nameof(website), LibraryManagementConsts.MaxUrlLength);
        }
        Website = website?.Trim();
        return this;
    }

    public Publisher SetContactEmail(string? contactEmail)
    {
        if (!string.IsNullOrWhiteSpace(contactEmail))
        {
            Check.Length(contactEmail, nameof(contactEmail), LibraryManagementConsts.MaxEmailLength);
        }
        ContactEmail = contactEmail?.Trim();
        return this;
    }

    public Publisher SetContactPhone(string? contactPhone)
    {
        if (!string.IsNullOrWhiteSpace(contactPhone))
        {
            Check.Length(contactPhone, nameof(contactPhone), LibraryManagementConsts.MaxPhoneNumberLength);
        }
        ContactPhone = contactPhone?.Trim();
        return this;
    }

    private void SetName(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Length(name, nameof(name), LibraryManagementConsts.MaxNameLength);
        Name = name.Trim();
    }
}