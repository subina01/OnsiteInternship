using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Domain.Events;

public class BasketClearedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public string CustomerId { get; }

    public BasketClearedEvent(string customerId)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        CustomerId = customerId;
    }
}
