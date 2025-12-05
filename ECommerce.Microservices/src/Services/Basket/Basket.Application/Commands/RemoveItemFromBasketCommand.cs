using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.Commands;

public class RemoveItemFromBasketCommand : IRequest<Unit>
{
    public string CustomerId { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
}
