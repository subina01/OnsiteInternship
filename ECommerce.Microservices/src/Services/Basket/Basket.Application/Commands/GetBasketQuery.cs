using Basket.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.Commands;

public class GetBasketQuery : IRequest<BasketDto?>
{
    public string CustomerId { get; set; } = string.Empty;
}
