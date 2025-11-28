using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace LibraryManagement.Domain.Repositories;

/// <summary>
/// Repository interface for Category aggregate
/// </summary>
public interface ICategoryRepository : IRepository<Category, Guid>
{
    Task<Category?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task<bool> HasBooksAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);
}
