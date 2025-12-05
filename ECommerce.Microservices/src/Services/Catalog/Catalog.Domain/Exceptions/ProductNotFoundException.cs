using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Domain.Exceptions;

public class ProductNotFoundException : CatalogDomainException
{
    public ProductNotFoundException(Guid productId)
        : base($"Product with id {productId} was not found.")
    { }
}
