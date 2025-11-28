using AutoMapper.Internal.Mappers;
using LibraryManagement.Application.Contracts.DTOs.Books;
using LibraryManagement.Application.Contracts.Permissions;
using LibraryManagement.Application.Contracts.Services;
using LibraryManagement.Domain.DomainServices;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using LibraryManagement.Domain.Shared.Constants;
using LibraryManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace LibraryManagement.Application.Services;

[Authorize(LibraryManagementPermissions.Books.Default)]
public class BookAppService : CrudAppService<
    Book,
    BookDto,
    Guid,
    GetBookListInput,
    CreateUpdateBookDto,
    CreateUpdateBookDto>, IBookAppService
{
    private readonly IBookRepository _bookRepository;
    private readonly IRepository<Author, Guid> _authorRepository;
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly BookManager _bookManager;

    public BookAppService(
        IBookRepository repository,
        IRepository<Author, Guid> authorRepository,
        IRepository<Category, Guid> categoryRepository,
        BookManager bookManager)
        : base(repository)
    {
        _bookRepository = repository;
        _authorRepository = authorRepository;
        _categoryRepository = categoryRepository;
        _bookManager = bookManager;

        GetPolicyName = LibraryManagementPermissions.Books.Default;
        GetListPolicyName = LibraryManagementPermissions.Books.Default;
        CreatePolicyName = LibraryManagementPermissions.Books.Create;
        UpdatePolicyName = LibraryManagementPermissions.Books.Edit;
        DeletePolicyName = LibraryManagementPermissions.Books.Delete;
    }

    protected override async Task<IQueryable<Book>> CreateFilteredQueryAsync(GetBookListInput input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        query = query
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                x => x.Title.Contains(input.Filter!) || x.ISBN.Value.Contains(input.Filter!))
            .WhereIf(input.AuthorId.HasValue,
                x => x.BookAuthors.Any(ba => ba.AuthorId == input.AuthorId!.Value))
            .WhereIf(input.CategoryId.HasValue,
                x => x.BookCategories.Any(bc => bc.CategoryId == input.CategoryId!.Value))
            .WhereIf(input.PublisherId.HasValue,
                x => x.PublisherId == input.PublisherId!.Value)
            .WhereIf(input.Status.HasValue,
                x => x.Status == input.Status!.Value)
            .WhereIf(input.PublicationYearFrom.HasValue,
                x => x.PublicationYear >= input.PublicationYearFrom!.Value)
            .WhereIf(input.PublicationYearTo.HasValue,
                x => x.PublicationYear <= input.PublicationYearTo!.Value)
            .WhereIf(!input.Language.IsNullOrWhiteSpace(),
                x => x.Language == input.Language)
            .WhereIf(input.AvailableOnly == true,
                x => x.AvailableQuantity > 0);

        return query;
    }

    public override async Task<BookDto> GetAsync(Guid id)
    {
        var query = await _bookRepository.WithDetailsAsync();
        var book = await AsyncExecuter.FirstOrDefaultAsync(
            query.Where(x => x.Id == id)
        );

        if (book == null)
        {
            throw new Volo.Abp.BusinessException(LibraryManagementErrorCodes.BookNotFound)
                .WithData("BookId", id);
        }

        return await MapToGetOutputDtoAsync(book);
    }

    [Authorize(LibraryManagementPermissions.Books.Create)]
    public override async Task<BookDto> CreateAsync(CreateUpdateBookDto input)
    {
        var book = await _bookManager.CreateAsync(
            input.Title,
            input.ISBN,
            input.PublicationYear,
            input.Edition,
            input.Quantity,
            input.PublisherId);

        await MapToEntityAsync(input, book);

        // Add authors
        foreach (var authorId in input.AuthorIds)
        {
            await _authorRepository.GetAsync(authorId); // Validate author exists
            book.AddAuthor(authorId);
        }

        // Add categories
        foreach (var categoryId in input.CategoryIds)
        {
            await _categoryRepository.GetAsync(categoryId); // Validate category exists
            book.AddCategory(categoryId);
        }

        await _bookRepository.InsertAsync(book);

        return await MapToGetOutputDtoAsync(book);
    }

    [Authorize(LibraryManagementPermissions.Books.Edit)]
    public override async Task<BookDto> UpdateAsync(Guid id, CreateUpdateBookDto input)
    {
        var book = await _bookRepository.GetAsync(id, includeDetails: true);

        // Update ISBN if changed
        if (book.ISBN.Value != input.ISBN)
        {
            await _bookManager.ChangeIsbnAsync(book, input.ISBN);
        }

        await MapToEntityAsync(input, book);

        // Update authors
        book.BookAuthors.Clear();
        foreach (var authorId in input.AuthorIds)
        {
            await _authorRepository.GetAsync(authorId);
            book.AddAuthor(authorId);
        }

        // Update categories
        book.BookCategories.Clear();
        foreach (var categoryId in input.CategoryIds)
        {
            await _categoryRepository.GetAsync(categoryId);
            book.AddCategory(categoryId);
        }

        await _bookRepository.UpdateAsync(book);

        return await MapToGetOutputDtoAsync(book);
    }

    protected override async Task MapToEntityAsync(CreateUpdateBookDto input, Book book)
    {
        book.SetTitle(input.Title)
            .SetPublicationYear(input.PublicationYear)
            .SetEdition(input.Edition)
            .SetDescription(input.Description)
            .SetLanguage(input.Language)
            .SetPageCount(input.PageCount)
            .SetQuantity(input.Quantity)
            .SetPublisher(input.PublisherId);
    }

    public async Task<List<BookDto>> GetAvailableBooksAsync()
    {
        var books = await _bookRepository.GetAvailableBooksAsync();
        return ObjectMapper.Map<List<Book>, List<BookDto>>(books);
    }

    public async Task<List<BookDto>> GetBooksByAuthorAsync(Guid authorId)
    {
        var books = await _bookRepository.GetListByAuthorIdAsync(authorId);
        return ObjectMapper.Map<List<Book>, List<BookDto>>(books);
    }

    public async Task<List<BookDto>> GetBooksByCategoryAsync(Guid categoryId)
    {
        var books = await _bookRepository.GetListByCategoryIdAsync(categoryId);
        return ObjectMapper.Map<List<Book>, List<BookDto>>(books);
    }

    public async Task<List<BookDto>> GetBooksByPublisherAsync(Guid publisherId)
    {
        var books = await _bookRepository.GetListByPublisherIdAsync(publisherId);
        return ObjectMapper.Map<List<Book>, List<BookDto>>(books);
    }

    public async Task<BookDto> GetByIsbnAsync(string isbn)
    {
        var isbnValue = ISBN.Create(isbn);
        var book = await _bookRepository.FindByIsbnAsync(isbnValue);

        if (book == null)
        {
            throw new Volo.Abp.BusinessException(LibraryManagementErrorCodes.BookNotFound)
                .WithData("ISBN", isbn);
        }

        return ObjectMapper.Map<Book, BookDto>(book);
    }
}
