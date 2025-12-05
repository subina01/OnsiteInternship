using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Domain.Events;

public class ProductUpdatedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid ProductId { get; }
    public string ProductName { get; }
    public decimal Price { get; }

    public ProductUpdatedEvent(Guid productId, string productName, decimal price)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        ProductId = productId;
        ProductName = productName;
        Price = price;
    }
}
