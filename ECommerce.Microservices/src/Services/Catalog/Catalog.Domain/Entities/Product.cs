using Common.Domain;
using Catalog.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Domain.Entities;

public class Product : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public bool IsAvailable { get; private set; }

    private Product() { } // EF Core

    public Product(string name, string description, decimal price, string imageUrl,
                   Guid categoryId, int stockQuantity, string sku)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));

        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));

        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        CategoryId = categoryId;
        StockQuantity = stockQuantity;
        SKU = sku;
        IsAvailable = true;

        AddDomainEvent(new ProductCreatedEvent(Id, Name, Price));
    }

    public void UpdateDetails(string name, string description, decimal price, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));

        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;

        AddDomainEvent(new ProductUpdatedEvent(Id, Name, Price));
    }

    public void UpdateStock(int quantity)
    {
        StockQuantity = quantity;
        IsAvailable = StockQuantity > 0;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (StockQuantity < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {StockQuantity}, Requested: {quantity}");

        StockQuantity -= quantity;
        IsAvailable = StockQuantity > 0;

        AddDomainEvent(new ProductStockReducedEvent(Id, quantity, StockQuantity));
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        StockQuantity += quantity;
        IsAvailable = true;
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }
}
