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

public class EfCorePublisherRepository : EfCoreRepository<LibraryManagementDbContext, Publisher, Guid>, IPublisherRepository
{
    public EfCorePublisherRepository(IDbContextProvider<LibraryManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Publisher?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(x => x.Name == name, GetCancellationToken(cancellationToken));
    }

    public async Task<bool> HasBooksAsync(
        Guid publisherId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(x => x.Books)
            .Where(x => x.Id == publisherId)
            .AnyAsync(x => x.Books.Any(), GetCancellationToken(cancellationToken));
    }

    public override async Task<IQueryable<Publisher>> WithDetailsAsync()
    {
        return (await GetQueryableAsync())
            .Include(x => x.Books);
    }
}
