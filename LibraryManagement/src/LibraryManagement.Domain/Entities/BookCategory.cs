using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// BookCategory - Junction entity for Book-Category many-to-many relationship
/// </summary>
public class BookCategory : Entity
{
    public Guid BookId { get; private set; }
    public Guid CategoryId { get; private set; }

    // Navigation properties
    public virtual Book Book { get; private set; } = null!;
    public virtual Category Category { get; private set; } = null!;

    private BookCategory()
    {
    }

    internal BookCategory(Guid bookId, Guid categoryId)
    {
        BookId = bookId;
        CategoryId = categoryId;
    }

    public override object[] GetKeys()
    {
        return new object[] { BookId, CategoryId };
    }
}
