using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Domain.Events;

public class OrderPaidEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid OrderId { get; }
    public string CustomerId { get; }

    public OrderPaidEvent(Guid orderId, string customerId)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        OrderId = orderId;
        CustomerId = customerId;
    }
}
