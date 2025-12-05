using MediatR;
using Order.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Queries;

public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    public Guid OrderId { get; set; }
}
