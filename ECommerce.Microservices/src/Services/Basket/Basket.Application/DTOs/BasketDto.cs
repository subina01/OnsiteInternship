using Basket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
namespace Basket.Application.DTOs;

public class BasketDto
{
    public string CustomerId { get; set; } = string.Empty;
    public List<BasketItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}
