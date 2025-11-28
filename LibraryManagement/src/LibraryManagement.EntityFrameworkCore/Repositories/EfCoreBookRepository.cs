using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using LibraryManagement.Domain.Shared.Enums;
using LibraryManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq;
using LibraryManagement.EntityFrameworkCore.EntityFrameworkCore;

namespace LibraryManagement.EntityFrameworkCore.Repositories;

public class EfCoreBookRepository : EfCoreRepository<LibraryManagementDbContext, Book, Guid>, IBookRepository
{
    public EfCoreBookRepository(IDbContextProvider<LibraryManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Book?> FindByIsbnAsync(ISBN isbn, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.ISBN.Value == isbn.Value)
            .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Book>> GetListByAuthorIdAsync(
        Guid authorId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(x => x.BookAuthors)
            .Where(x => x.BookAuthors.Any(ba => ba.AuthorId == authorId))
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Book>> GetListByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(x => x.BookCategories)
            .Where(x => x.BookCategories.Any(bc => bc.CategoryId == categoryId))
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Book>> GetListByPublisherIdAsync(
        Guid publisherId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.PublisherId == publisherId)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Book>> GetAvailableBooksAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Status == BookStatus.Available && x.AvailableQuantity > 0)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> IsIsbnExistsAsync(
        ISBN isbn,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.Where(x => x.ISBN.Value == isbn.Value);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public override async Task<IQueryable<Book>> WithDetailsAsync()
    {
        return (await GetQueryableAsync())
            .Include(x => x.Publisher)
            .Include(x => x.BookAuthors).ThenInclude(x => x.Author)
            .Include(x => x.BookCategories).ThenInclude(x => x.Category);
    }
}
