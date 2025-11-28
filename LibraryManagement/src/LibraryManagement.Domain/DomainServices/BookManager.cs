using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using LibraryManagement.Domain.Shared.Constants;
using LibraryManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace LibraryManagement.Domain.DomainServices;

/// <summary>
/// Domain Service for managing Book business logic
/// </summary>
public class BookManager : DomainService
{
    private readonly IBookRepository _bookRepository;

    public BookManager(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Book> CreateAsync(
        string title,
        string isbn,
        int publicationYear,
        int edition,
        int quantity,
        Guid? publisherId = null)
    {
        Check.NotNullOrWhiteSpace(title, nameof(title));
        Check.NotNullOrWhiteSpace(isbn, nameof(isbn));

        var isbnValue = ISBN.Create(isbn);

        // Check if ISBN already exists
        if (await _bookRepository.IsIsbnExistsAsync(isbnValue))
        {
            throw new BusinessException(LibraryManagementErrorCodes.DuplicateIsbn)
                .WithData("ISBN", isbn);
        }

        var book = new Book(
            GuidGenerator.Create(),
            title,
            isbnValue,
            publicationYear,
            edition,
            quantity,
            publisherId);

        return book;
    }

    public async Task ChangeIsbnAsync(Book book, string newIsbn)
    {
        Check.NotNull(book, nameof(book));
        Check.NotNullOrWhiteSpace(newIsbn, nameof(newIsbn));

        var isbnValue = ISBN.Create(newIsbn);

        // Check if new ISBN already exists for a different book
        if (await _bookRepository.IsIsbnExistsAsync(isbnValue, book.Id))
        {
            throw new BusinessException(LibraryManagementErrorCodes.DuplicateIsbn)
                .WithData("ISBN", newIsbn);
        }

        book.SetISBN(isbnValue);
    }

    public void ValidateBookAvailability(Book book)
    {
        Check.NotNull(book, nameof(book));

        if (!book.IsAvailableForLoan())
        {
            throw new BusinessException(LibraryManagementErrorCodes.BookNotAvailable)
                .WithData("BookId", book.Id)
                .WithData("Status", book.Status)
                .WithData("AvailableQuantity", book.AvailableQuantity);
        }
    }
}
