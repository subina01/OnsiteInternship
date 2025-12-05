using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Domain.Events;

public class BasketItemRemovedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string CustomerId { get; }
    public Guid ProductId { get; }

    public BasketItemRemovedEvent(string customerId, Guid productId)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        CustomerId = customerId;
        ProductId = productId;
    }
}

