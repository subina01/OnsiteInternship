using LibraryManagement.Domain.Shared.Constants;
using LibraryManagement.Domain.Shared.Enums;
using LibraryManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace LibraryManagement.Domain.Entities;


/// <summary>
/// Book Aggregate Root - Main entity for book management
/// </summary>
public class Book : FullAuditedAggregateRoot<Guid>
{
    public string Title { get; private set; }
    public ISBN ISBN { get; private set; }
    public int PublicationYear { get; private set; }
    public int Edition { get; private set; }
    public string? Description { get; private set; }
    public string? Language { get; private set; }
    public int? PageCount { get; private set; }
    public int Quantity { get; private set; }
    public int AvailableQuantity { get; private set; }
    public BookStatus Status { get; private set; }

    // Foreign Keys
    public Guid? PublisherId { get; private set; }

    // Navigation Properties
    public virtual Publisher? Publisher { get; private set; }
    public virtual ICollection<BookAuthor> BookAuthors { get; private set; }
    public virtual ICollection<BookCategory> BookCategories { get; private set; }

    private Book()
    {
        Title = string.Empty;
        ISBN = null!;
        BookAuthors = new List<BookAuthor>();
        BookCategories = new List<BookCategory>();
    }

    internal Book(
        Guid id,
        string title,
        ISBN isbn,
        int publicationYear,
        int edition,
        int quantity,
        Guid? publisherId = null) : base(id)
    {
        Title = string.Empty; // Ensure non-null before calling SetTitle
        ISBN = null!;         // Ensure non-null before calling SetISBN
        SetTitle(title);
        SetISBN(isbn);
        SetPublicationYear(publicationYear);
        SetEdition(edition);
        SetQuantity(quantity);
        PublisherId = publisherId;
        AvailableQuantity = quantity;
        Status = BookStatus.Available;
        BookAuthors = new List<BookAuthor>();
        BookCategories = new List<BookCategory>();
    }

    public Book SetTitle(string title)
    {
        Check.NotNullOrWhiteSpace(title, nameof(title));
        Check.Length(title, nameof(title), LibraryManagementConsts.MaxTitleLength);
        Title = title.Trim();
        return this;
    }

    public Book SetISBN(ISBN isbn)
    {
        Check.NotNull(isbn, nameof(isbn));
        ISBN = isbn;
        return this;
    }

    public Book SetPublicationYear(int publicationYear)
    {
        if (publicationYear < LibraryManagementConsts.Books.MinPublicationYear ||
            publicationYear > DateTime.UtcNow.Year)
        {
            throw new BusinessException(LibraryManagementErrorCodes.ValidationError)
                .WithData("Field", "PublicationYear")
                .WithData("Message", $"Publication year must be between {LibraryManagementConsts.Books.MinPublicationYear} and {DateTime.UtcNow.Year}");
        }
        PublicationYear = publicationYear;
        return this;
    }

    public Book SetEdition(int edition)
    {
        if (edition < 1 || edition > LibraryManagementConsts.Books.MaxEdition)
        {
            throw new BusinessException(LibraryManagementErrorCodes.ValidationError)
                .WithData("Field", "Edition")
                .WithData("Message", $"Edition must be between 1 and {LibraryManagementConsts.Books.MaxEdition}");
        }
        Edition = edition;
        return this;
    }

    public Book SetDescription(string? description)
    {
        if (!string.IsNullOrWhiteSpace(description))
        {
            Check.Length(description, nameof(description), LibraryManagementConsts.MaxDescriptionLength);
        }
        Description = description?.Trim();
        return this;
    }

    public Book SetLanguage(string? language)
    {
        if (!string.IsNullOrWhiteSpace(language))
        {
            Check.Length(language, nameof(language), LibraryManagementConsts.MaxShortTextLength);
        }
        Language = language?.Trim();
        return this;
    }

    public Book SetPageCount(int? pageCount)
    {
        if (pageCount.HasValue && (pageCount.Value < LibraryManagementConsts.Books.MinPageCount ||
            pageCount.Value > LibraryManagementConsts.Books.MaxPageCount))
        {
            throw new BusinessException(LibraryManagementErrorCodes.ValidationError)
                .WithData("Field", "PageCount")
                .WithData("Message", $"Page count must be between {LibraryManagementConsts.Books.MinPageCount} and {LibraryManagementConsts.Books.MaxPageCount}");
        }
        PageCount = pageCount;
        return this;
    }

    public Book SetQuantity(int quantity)
    {
        if (quantity < 0 || quantity > LibraryManagementConsts.Books.MaxQuantity)
        {
            throw new BusinessException(LibraryManagementErrorCodes.ValidationError)
                .WithData("Field", "Quantity")
                .WithData("Message", $"Quantity must be between 0 and {LibraryManagementConsts.Books.MaxQuantity}");
        }

        var difference = quantity - Quantity;
        Quantity = quantity;
        AvailableQuantity += difference;

        UpdateStatus();
        return this;
    }

    public Book SetPublisher(Guid? publisherId)
    {
        PublisherId = publisherId;
        return this;
    }

    public void AddAuthor(Guid authorId)
    {
        if (BookAuthors.Any(ba => ba.AuthorId == authorId))
        {
            return;
        }

        BookAuthors.Add(new BookAuthor(Id, authorId));
    }

    public void RemoveAuthor(Guid authorId)
    {
        var bookAuthor = BookAuthors.FirstOrDefault(ba => ba.AuthorId == authorId);
        if (bookAuthor != null)
        {
            BookAuthors.Remove(bookAuthor);
        }
    }

    public void AddCategory(Guid categoryId)
    {
        if (BookCategories.Any(bc => bc.CategoryId == categoryId))
        {
            return;
        }

        BookCategories.Add(new BookCategory(Id, categoryId));
    }

    public void RemoveCategory(Guid categoryId)
    {
        var bookCategory = BookCategories.FirstOrDefault(bc => bc.CategoryId == categoryId);
        if (bookCategory != null)
        {
            BookCategories.Remove(bookCategory);
        }
    }

    public void DecreaseAvailableQuantity(int count = 1)
    {
        if (AvailableQuantity < count)
        {
            throw new BusinessException(LibraryManagementErrorCodes.InsufficientBookQuantity)
                .WithData("BookId", Id)
                .WithData("RequiredQuantity", count)
                .WithData("AvailableQuantity", AvailableQuantity);
        }

        AvailableQuantity -= count;
        UpdateStatus();
    }

    public void IncreaseAvailableQuantity(int count = 1)
    {
        AvailableQuantity += count;

        if (AvailableQuantity > Quantity)
        {
            AvailableQuantity = Quantity;
        }

        UpdateStatus();
    }

    public void SetStatus(BookStatus status)
    {
        Status = status;
    }

    private void UpdateStatus()
    {
        if (Status == BookStatus.Lost || Status == BookStatus.Damaged || Status == BookStatus.Maintenance)
        {
            return;
        }

        Status = AvailableQuantity > 0 ? BookStatus.Available : BookStatus.CheckedOut;
    }

    public bool IsAvailableForLoan()
    {
        return Status == BookStatus.Available && AvailableQuantity > 0;
    }
}
