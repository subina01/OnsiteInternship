using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Domain.Events;

public class OrderShippedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid OrderId { get; }
    public string CustomerId { get; }
    public string TrackingNumber { get; }

    public OrderShippedEvent(Guid orderId, string customerId, string trackingNumber)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        OrderId = orderId;
        CustomerId = customerId;
        TrackingNumber = trackingNumber;
    }
}
