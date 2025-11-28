using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Volo.Abp.Specifications;

namespace LibraryManagement.Domain.Specifications;

public class BookByAuthorSpecification : Specification<Book>
{
    private readonly Guid _authorId;

    public BookByAuthorSpecification(Guid authorId)
    {
        _authorId = authorId;
    }

    public override Expression<Func<Book, bool>> ToExpression()
    {
        return BookSpecifications.ByAuthorId(_authorId);
    }
}
