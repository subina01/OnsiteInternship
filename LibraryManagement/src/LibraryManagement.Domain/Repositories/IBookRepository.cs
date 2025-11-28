using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace LibraryManagement.Domain.Repositories;

/// <summary>
/// Repository interface for Book aggregate
/// </summary>
public interface IBookRepository : IRepository<Book, Guid>
{
    Task<Book?> FindByIsbnAsync(ISBN isbn, CancellationToken cancellationToken = default);

    Task<List<Book>> GetListByAuthorIdAsync(
        Guid authorId,
        CancellationToken cancellationToken = default);

    Task<List<Book>> GetListByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);

    Task<List<Book>> GetListByPublisherIdAsync(
        Guid publisherId,
        CancellationToken cancellationToken = default);

    Task<List<Book>> GetAvailableBooksAsync(
        CancellationToken cancellationToken = default);

    Task<bool> IsIsbnExistsAsync(
        ISBN isbn,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
} 
