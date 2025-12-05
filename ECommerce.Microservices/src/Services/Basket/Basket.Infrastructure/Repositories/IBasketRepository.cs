using Basket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Infrastructure.Repositories;

public interface IBasketRepository
{
    Task<CustomerBasket?> GetBasketAsync(string customerId, CancellationToken cancellationToken = default);
    Task<CustomerBasket> CreateOrUpdateBasketAsync(CustomerBasket basket, CancellationToken cancellationToken = default);
    Task<bool> DeleteBasketAsync(string customerId, CancellationToken cancellationToken = default);
}
