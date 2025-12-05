using EventBus.RabbitMQ.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Infrastructure.EventBus;

public record BasketCheckoutIntegrationEvent : IntegrationEvent
{
    public string CustomerId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public decimal TotalPrice { get; init; }
    public List<BasketCheckoutItem> Items { get; init; } = new();
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
}
public record BasketCheckoutItem
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
}
