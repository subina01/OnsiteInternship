using Common.Domain;
using Order.Domain.Events;
using Order.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Order.Domain.Entities;

public class Order : BaseEntity, IAggregateRoot
{
    public string CustomerId { get; private set; } = string.Empty;
    public Address ShippingAddress { get; private set; }
    public Address BillingAddress { get; private set; }
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount => _orderItems.Sum(i => i.Price * i.Quantity);
    public DateTime? OrderDate { get; private set; }
    public DateTime? ShippedDate { get; private set; }
    public DateTime? DeliveredDate { get; private set; }
    public string? TrackingNumber { get; private set; } = string.Empty;

    private Order() { } // EF Core

    public Order(string customerId, Address shippingAddress, Address billingAddress)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        CustomerId = customerId;
        ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
        BillingAddress = billingAddress ?? throw new ArgumentNullException(nameof(billingAddress));
        Status = OrderStatus.Pending;
        OrderDate = DateTime.UtcNow;

        AddDomainEvent(new OrderCreatedEvent(Id, CustomerId, TotalAmount));
    }

    public void AddOrderItem(Guid productId, string productName, decimal price, int quantity)
    {
        var existingItem = _orderItems.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity);
        }
        else
        {
            _orderItems.Add(new OrderItem(productId, productName, price, quantity));
        }
    }

    public void SetPaidStatus()
    {
        if (Status == OrderStatus.Pending)
        {
            Status = OrderStatus.Paid;
            AddDomainEvent(new OrderPaidEvent(Id, CustomerId));
        }
    }

    public void SetShippedStatus(string trackingNumber)
    {
        if (Status == OrderStatus.Paid)
        {
            Status = OrderStatus.Shipped;
            ShippedDate = DateTime.UtcNow;
            TrackingNumber = trackingNumber;
            AddDomainEvent(new OrderShippedEvent(Id, CustomerId, trackingNumber));
        }
    }

    public void SetDeliveredStatus()
    {
        if (Status == OrderStatus.Shipped)
        {
            Status = OrderStatus.Delivered;
            DeliveredDate = DateTime.UtcNow;
            AddDomainEvent(new OrderDeliveredEvent(Id, CustomerId));
        }
    }

    public void CancelOrder(string reason)
    {
        if (Status == OrderStatus.Pending || Status == OrderStatus.Paid)
        {
            Status = OrderStatus.Cancelled;
            AddDomainEvent(new OrderCancelledEvent(Id, CustomerId, reason));
        }
        else
        {
            throw new InvalidOperationException($"Cannot cancel order in {Status} status");
        }
    }
}
