using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Domain.Events;

public class BasketItemAddedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string CustomerId { get; }
    public Guid ProductId { get; }
    public int Quantity { get; }

    public BasketItemAddedEvent(string customerId, Guid productId, int quantity)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        CustomerId = customerId;
        ProductId = productId;
        Quantity = quantity;
    }
}
