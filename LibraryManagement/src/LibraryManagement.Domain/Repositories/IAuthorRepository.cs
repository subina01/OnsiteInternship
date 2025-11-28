using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace LibraryManagement.Domain.Repositories;

/// <summary>
/// Repository interface for Author aggregate
/// </summary>
public interface IAuthorRepository : IRepository<Author, Guid>
{
    Task<Author?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task<List<Author>> GetListByNationalityAsync(
        string nationality,
        CancellationToken cancellationToken = default);

    Task<bool> HasBooksAsync(
        Guid authorId,
        CancellationToken cancellationToken = default);
}
