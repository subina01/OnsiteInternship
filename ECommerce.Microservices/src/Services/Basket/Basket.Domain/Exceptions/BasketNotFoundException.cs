using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Domain.Exceptions;

public class BasketNotFoundException : BasketDomainException
{
    public BasketNotFoundException(string customerId)
        : base($"Basket for customer {customerId} was not found.")
    { }
}
