using MediatR;
using Order.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Queries;

public class GetOrdersByCustomerQuery : IRequest<List<OrderDto>>
{
    public string CustomerId { get; set; } = string.Empty;
}
