using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Domain.Events;

public class ProductStockReducedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid ProductId { get; }
    public int QuantityReduced { get; }
    public int RemainingStock { get; }

    public ProductStockReducedEvent(Guid productId, int quantityReduced, int remainingStock)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        ProductId = productId;
        QuantityReduced = quantityReduced;
        RemainingStock = remainingStock;
    }
}
