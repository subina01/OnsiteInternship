using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Volo.Abp.Specifications;

namespace LibraryManagement.Domain.Specifications;

public class LowStockBookSpecification : Specification<Book>
{
    private readonly int _threshold;

    public LowStockBookSpecification(int threshold = 2)
    {
        _threshold = threshold;
    }

    public override Expression<Func<Book, bool>> ToExpression()
    {
        return BookSpecifications.LowStock(_threshold);
    }
}
