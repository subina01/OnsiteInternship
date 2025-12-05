using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Infrastructure.Repositories;

public interface IOrderRepository
{
    Task<Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Order>> GetOrdersByCustomerAsync(string customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Domain.Entities.Order> Orders, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Entities.Order order, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.Order order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Domain.Entities.Order order, CancellationToken cancellationToken = default);
}
