using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Application.Commands;

public class CheckoutBasketCommand : IRequest<bool>
{
    public string CustomerId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}
