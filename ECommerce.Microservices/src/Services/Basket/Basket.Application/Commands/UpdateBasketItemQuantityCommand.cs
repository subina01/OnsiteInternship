using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.Commands;

public class UpdateBasketItemQuantityCommand : IRequest<Unit>
{
    public string CustomerId { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
