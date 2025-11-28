using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Volo.Abp.Specifications;

namespace LibraryManagement.Domain.Specifications;

/// <summary>
/// Composable specifications using ABP Specification pattern
/// </summary>
public class AvailableBookSpecification : Specification<Book>
{
    public override Expression<Func<Book, bool>> ToExpression()
    {
        return BookSpecifications.IsAvailable();
    }
}