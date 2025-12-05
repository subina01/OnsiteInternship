using Basket.Domain.Events;
using Common.Domain;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Domain.Entities;

public class CustomerBasket : BaseEntity, IAggregateRoot
{
    public string CustomerId { get; private set; }
    [JsonInclude]
    private List<BasketItem> _items = new();
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();
    public decimal TotalPrice => _items.Sum(i => i.Price * i.Quantity);

    private CustomerBasket() { _items = new List<BasketItem>(); } // For serialization

    public CustomerBasket(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        CustomerId = customerId;
    }

    public void AddItem(Guid productId, string productName, decimal price, int quantity, string imageUrl)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(new BasketItem(productId, productName, price, quantity, imageUrl));
        }

        AddDomainEvent(new BasketItemAddedEvent(CustomerId, productId, quantity));
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new InvalidOperationException($"Product {productId} not found in basket");

        if (quantity <= 0)
        {
            RemoveItem(productId);
        }
        else
        {
            item.UpdateQuantity(quantity);
        }
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _items.Remove(item);
            AddDomainEvent(new BasketItemRemovedEvent(CustomerId, productId));
        }
    }

    public void ClearBasket()
    {
        _items.Clear();
        AddDomainEvent(new BasketClearedEvent(CustomerId));
    }
}

