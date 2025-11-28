using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.EntityFrameworkCore.EntityFrameworkCore;


namespace LibraryManagement.EntityFrameworkCore.Repositories;

public class EfCoreAuthorRepository : EfCoreRepository<LibraryManagementDbContext, Author, Guid>, IAuthorRepository
{
    public EfCoreAuthorRepository(IDbContextProvider<LibraryManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Author?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(x => x.Name == name, GetCancellationToken(cancellationToken));
    }

    public async Task<List<Author>> GetListByNationalityAsync(
        string nationality,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Nationality == nationality)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> HasBooksAsync(
        Guid authorId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(x => x.BookAuthors)
            .Where(x => x.Id == authorId)
            .AnyAsync(x => x.BookAuthors.Any(), GetCancellationToken(cancellationToken));
    }

    public override async Task<IQueryable<Author>> WithDetailsAsync()
    {
        return (await GetQueryableAsync())
            .Include(x => x.BookAuthors).ThenInclude(x => x.Book);
    }
}
