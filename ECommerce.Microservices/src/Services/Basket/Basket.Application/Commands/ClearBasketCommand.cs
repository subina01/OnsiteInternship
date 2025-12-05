using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.Commands;

public class ClearBasketCommand : IRequest<Unit>
{
    public string CustomerId { get; set; } = string.Empty;
}
