using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace LibraryManagement.Domain.Repositories;

/// <summary>
/// Repository interface for Publisher aggregate
/// </summary>
public interface IPublisherRepository : IRepository<Publisher, Guid>
{
    Task<Publisher?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task<bool> HasBooksAsync(
        Guid publisherId,
        CancellationToken cancellationToken = default);
}
