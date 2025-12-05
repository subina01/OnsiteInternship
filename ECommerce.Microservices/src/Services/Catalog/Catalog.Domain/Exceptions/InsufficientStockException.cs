using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Domain.Exceptions;

public class InsufficientStockException : CatalogDomainException
{
    public InsufficientStockException(Guid productId, int requested, int available)
        : base($"Product {productId} has insufficient stock. Requested: {requested}, Available: {available}")
    { }
}
