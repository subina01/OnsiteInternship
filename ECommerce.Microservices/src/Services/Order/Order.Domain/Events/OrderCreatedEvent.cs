using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Domain.Events;

public class OrderCreatedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid OrderId { get; }
    public string CustomerId { get; }
    public decimal TotalAmount { get; }

    public OrderCreatedEvent(Guid orderId, string customerId, decimal totalAmount)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
    }
}
