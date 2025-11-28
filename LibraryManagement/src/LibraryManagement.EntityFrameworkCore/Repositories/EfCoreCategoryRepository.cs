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

public class EfCoreCategoryRepository : EfCoreRepository<LibraryManagementDbContext, Category, Guid>, ICategoryRepository
{
    public EfCoreCategoryRepository(IDbContextProvider<LibraryManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Category?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(x => x.Name == name, GetCancellationToken(cancellationToken));
    }

    public async Task<bool> HasBooksAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(x => x.BookCategories)
            .Where(x => x.Id == categoryId)
            .AnyAsync(x => x.BookCategories.Any(), GetCancellationToken(cancellationToken));
    }

    public override async Task<IQueryable<Category>> WithDetailsAsync()
    {
        return (await GetQueryableAsync())
            .Include(x => x.BookCategories).ThenInclude(x => x.Book);
    }
}
