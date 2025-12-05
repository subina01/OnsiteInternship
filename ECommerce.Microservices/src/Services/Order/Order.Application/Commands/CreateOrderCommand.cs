using MediatR;
using Order.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Commands;

public class CreateOrderCommand : IRequest<Guid>
{
    public string CustomerId { get; set; } = string.Empty;
    public AddressDto ShippingAddress { get; set; } = null!;
    public AddressDto BillingAddress { get; set; } = null!;
    public List<OrderItemDto> Items { get; set; } = new();
}
