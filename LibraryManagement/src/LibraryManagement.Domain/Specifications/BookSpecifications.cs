using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LibraryManagement.Domain.Specifications;

/// <summary>
/// Specifications for Book entity - following Specification Pattern
/// </summary>
public static class BookSpecifications
{
    /// <summary>
    /// Specification for available books
    /// </summary>
    public static Expression<Func<Book, bool>> IsAvailable()
    {
        return book => book.Status == BookStatus.Available && book.AvailableQuantity > 0;
    }

    /// <summary>
    /// Specification for books by author
    /// </summary>
    public static Expression<Func<Book, bool>> ByAuthorId(Guid authorId)
    {
        return book => book.BookAuthors.Any(ba => ba.AuthorId == authorId);
    }

    /// <summary>
    /// Specification for books by category
    /// </summary>
    public static Expression<Func<Book, bool>> ByCategoryId(Guid categoryId)
    {
        return book => book.BookCategories.Any(bc => bc.CategoryId == categoryId);
    }

    /// <summary>
    /// Specification for books by publisher
    /// </summary>
    public static Expression<Func<Book, bool>> ByPublisherId(Guid publisherId)
    {
        return book => book.PublisherId == publisherId;
    }

    /// <summary>
    /// Specification for books published in a year range
    /// </summary>
    public static Expression<Func<Book, bool>> PublishedBetween(int startYear, int endYear)
    {
        return book => book.PublicationYear >= startYear && book.PublicationYear <= endYear;
    }

    /// <summary>
    /// Specification for books by language
    /// </summary>
    public static Expression<Func<Book, bool>> ByLanguage(string language)
    {
        return book => book.Language == language;
    }

    /// <summary>
    /// Specification for books with low stock
    /// </summary>
    public static Expression<Func<Book, bool>> LowStock(int threshold = 2)
    {
        return book => book.AvailableQuantity > 0 && book.AvailableQuantity <= threshold;
    }

    /// <summary>
    /// Specification for books out of stock
    /// </summary>
    public static Expression<Func<Book, bool>> OutOfStock()
    {
        return book => book.AvailableQuantity == 0 && book.Status == BookStatus.CheckedOut;
    }
}
