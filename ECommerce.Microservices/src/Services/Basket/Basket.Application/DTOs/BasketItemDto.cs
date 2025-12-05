using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.DTOs;

public class BasketItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

