using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Domain.Exceptions;

public class OrderNotFoundException : OrderDomainException
{
    public OrderNotFoundException(Guid orderId)
        : base($"Order with id {orderId} was not found.")
    { }
}
